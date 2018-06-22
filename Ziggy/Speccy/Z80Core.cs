//Z80Core.cs
//(c) Arjun Nair 2009

namespace Speccy
{
    public class Z80Core
    {
        public bool loggingEnabled = false;
        public bool runningInterrupt = false;   //true if interrupts are active
        public bool and_32_Or_64;       //used for edge loading
        public bool resetOver = false;
        public bool HaltOn = false;             //true if HALT instruction is being processed
        public byte lastOpcodeWasEI = 0;        //used for re-triggered interrupts
        public int tstates = 0;                 //opcode t-states
        public int totalTStates = 0;
        public int oldTStates = 0;
        public int interruptMode;               //0 = IM0, 1 = IM1, 2 = IM2
        public bool IFF1, IFF2;

        protected int frameCount;
        protected int disp = 0;                 //used later on to calculate relative jumps in Execute()
        protected int deltaTStates = 0;
        protected int timeToOutSound = 0;

        //All registers
        protected int a, f, bc, hl, de, sp, pc, ix, iy;
        protected int i, r;

        //All alternate registers
        protected int _af, _bc, _de, _hl;
        protected int _r;                   //not really a real z80 alternate reg, but used here to store the value for R temporarily

        //MEMPTR register - internal cpu register
        //Bits 3 and 5 of Flag for Bit n, (HL) instruction, are copied from bits 11 & 13 of MemPtr.
        protected int memPtr;
        public int MemPtr {
            get => memPtr;
            set => memPtr = value & 0xffff;
        }

        protected const int MEMPTR_11 = 0x800;
        protected const int MEMPTR_13 = 0x2000;
        protected const int F_CARRY = 0x01;
        protected const int F_NEG = 0x02;
        protected const int F_PARITY = 0x04;
        protected const int F_3 = 0x08;
        protected const int F_HALF = 0x010;
        protected const int F_5 = 0x020;
        protected const int F_ZERO = 0x040;
        protected const int F_SIGN = 0x080;

        //Tables for parity and flags. Pretty much taken from Fuse.
        protected byte[] parity = new byte[256];
        protected byte[] IOIncParityTable = { 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0 };
        protected byte[] IODecParityTable = { 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1 };
        protected byte[] halfcarry_add = { 0, F_HALF, F_HALF, F_HALF, 0, 0, 0, F_HALF };
        protected byte[] halfcarry_sub = { 0, 0, F_HALF, 0, F_HALF, 0, F_HALF, F_HALF };
        protected byte[] overflow_add = { 0, 0, 0, F_PARITY, F_PARITY, 0, 0, 0 };
        protected byte[] overflow_sub = { 0, F_PARITY, 0, 0, 0, 0, F_PARITY, 0 };
        protected byte[] sz53 = new byte[256];
        protected byte[] sz53p = new byte[256];

        #region 8 bit register access

        public int A {
            get => a & 0xff;
            set => a = value;
        }

        public int B {
            get => (bc >> 8) & 0xff;
            set => bc = (bc & 0x00ff) | (value << 8);
        }

        public int C {
            get => bc & 0xff;
            set => bc = (bc & 0xff00) | value;
        }

        public int H {
            get => (hl >> 8) & 0xff;
            set => hl = (hl & 0x00ff) | (value << 8);
        }

        public int L {
            get => hl & 0xff;
            set => hl = (hl & 0xff00) | value;
        }

        public int D {
            get => (de >> 8) & 0xff;
            set => de = (de & 0x00ff) | (value << 8);
        }

        public int E {
            get => de & 0xff;
            set => de = (de & 0xff00) | value;
        }

        public int F {
            get => f & 0xff;
            set => f = value;
        }

        public int I {
            get => i & 0xff;
            set => i = value;
        }

        public int R {
            get => _r | (r & 0x7f);
            set => r = value & 0x7f;
        }

        public int _R {
            set {
                _r = value & 0x80;  //store Bit 7
                R = value;
            }
        }

        public int IXH {
            get => (ix >> 8) & 0xff;
            set => ix = (ix & 0x00ff) | (value << 8);
        }

        public int IXL {
            get => ix & 0xff;
            set => ix = (ix & 0xff00) | value;
        }

        public int IYH {
            get => (iy >> 8) & 0xff;
            set => iy = (iy & 0x00ff) | (value << 8);
        }

        public int IYL {
            get => iy & 0xff;
            set => iy = (iy & 0xff00) | value;
        }

        #endregion 8 bit register access

        #region 16 bit register access

        public int IR => (I << 8) | R;

        public int AF {
            get => (a << 8) | f;
            set {
                a = (value & 0xff00) >> 8;
                f = value & 0x00ff;
            }
        }

        public int _AF {
            get => _af;
            set => _af = value;
        }

        public int _HL {
            get => _hl;
            set => _hl = value;
        }

        public int _BC {
            get => _bc;
            set => _bc = value;
        }

        public int _DE {
            get => _de;
            set => _de = value;
        }

        public int BC {
            get => bc & 0xffff;
            set => bc = value & 0xffff;
        }

        public int DE {
            get => de & 0xffff;
            set => de = value & 0xffff;
        }

        public int HL {
            get => hl & 0xffff;
            set => hl = value & 0xffff;
        }

        public int IX {
            get => ix & 0xffff;
            set => ix = value & 0xffff;
        }

        public int IY {
            get => iy & 0xffff;
            set => iy = value & 0xffff;
        }

        public int SP {
            get => sp & 0xffff;
            set => sp = value & 0xffff;
        }

        public int PC {
            get => pc & 0xffff;
            set => pc = value & 0xffff;
        }

        #endregion 16 bit register access

        #region Flag manipulation

        public void SetCarry(bool val) {
            if (val) {
                f |= F_CARRY;
            }
            else {
                f &= ~F_CARRY;
            }
        }

        public void SetNeg(bool val) {
            if (val) {
                f |= F_NEG;
            }
            else {
                f &= ~F_NEG;
            }
        }

        public void SetParity(byte val) {
            if (val > 0) {
                f |= F_PARITY;
            }
            else {
                f &= ~F_PARITY;
            }
        }

        public void SetParity(bool val) {
            if (val) {
                f |= F_PARITY;
            }
            else {
                f &= ~F_PARITY;
            }
        }

        public void SetHalf(bool val) {
            if (val) {
                f |= F_HALF;
            }
            else {
                f &= ~F_HALF;
            }
        }

        public void SetZero(bool val) {
            if (val) {
                f |= F_ZERO;
            }
            else {
                f &= ~F_ZERO;
            }
        }

        public void SetSign(bool val) {
            if (val) {
                f |= F_SIGN;
            }
            else {
                f &= ~F_SIGN;
            }
        }

        public void SetF3(bool val) {
            if (val) {
                f |= F_3;
            }
            else {
                f &= ~F_3;
            }
        }

        public void SetF5(bool val) {
            if (val) {
                f |= F_5;
            }
            else {
                f &= ~F_5;
            }
        }

        #endregion Flag manipulation

        public Z80Core() {
            for (int i = 0; i < 256; i++) {
                sz53[i] = (byte)(i & (F_3 | F_5 | F_SIGN));
                var j = i; byte p = 0;
                for (int k = 0; k < 8; k++) { p ^= (byte)(j & 1); j >>= 1; }
                parity[i] = (byte)(p > 0 ? 0 : F_PARITY);
                sz53p[i] = (byte)(sz53[i] | parity[i]);
            }

            sz53[0] |= F_ZERO;
            sz53p[0] |= F_ZERO;
        }

        public void exx() {
            int temp = _hl;
            _hl = HL;
            HL = temp;

            temp = _de;
            _de = DE;
            DE = temp;

            temp = _bc;
            _bc = BC;
            BC = temp;
        }

        public void ex_af_af() {
            int temp = _af;
            _af = AF;
            AF = temp;
        }

        public int Inc(int reg) {
            reg = reg + 1;
            F = (F & F_CARRY) | (reg == 0x80 ? F_PARITY : 0) | ((reg & 0x0f) > 0 ? 0 : F_HALF);
            reg &= 0xff;
            F |= sz53[reg];
            return reg;
        }

        public int Dec(int reg) {
            F = (F & F_CARRY) | ((reg & 0x0f) > 0 ? 0 : F_HALF) | F_NEG;
            reg = reg - 1;
            F |= reg == 0x7f ? F_PARITY : 0;
            reg &= 0xff;
            F |= sz53[reg];
            return reg;
        }

        //16 bit addition (no carry)
        public int Add_RR(int rr1, int rr2) {
            int add16temp = rr1 + rr2;
            byte lookup = (byte)(((rr1 & 0x0800) >> 11) | ((rr2 & 0x0800) >> 10) | ((add16temp & 0x0800) >> 9));
            rr1 = add16temp;
            F = (F & (F_PARITY | F_ZERO | F_SIGN)) | ((add16temp & 0x10000) > 0 ? F_CARRY : 0) | ((add16temp >> 8) & (F_3 | F_5)) | halfcarry_add[lookup];
            return rr1 & 0xffff;
        }

        //8 bit add to accumulator (no carry)
        public void Add_R(int reg) {
            int addtemp = A + reg;
            byte lookup = (byte)(((A & 0x88) >> 3) | ((reg & 0x88) >> 2) | ((addtemp & 0x88) >> 1));
            A = addtemp & 0xff;
            F = ((addtemp & 0x100) > 0 ? F_CARRY : 0) | halfcarry_add[lookup & 0x07] | overflow_add[lookup >> 4] | sz53[A];
        }

        //Add with carry into accumulator
        public void Adc_R(int reg) {
            int adctemp = A + reg + (F & F_CARRY);
            byte lookup = (byte)(((A & 0x88) >> 3) | ((reg & 0x88) >> 2) | ((adctemp & 0x88) >> 1));
            A = adctemp & 0xff;
            F = ((adctemp & 0x100) > 0 ? F_CARRY : 0) | halfcarry_add[lookup & 0x07] | overflow_add[lookup >> 4] | sz53[A];
        }

        //Add with carry into HL
        public void Adc_RR(int reg) {
            int add16temp = HL + reg + (F & F_CARRY);
            byte lookup = (byte)(((HL & 0x8800) >> 11) | ((reg & 0x8800) >> 10) | ((add16temp & 0x8800) >> 9));
            HL = add16temp & 0xffff;
            F = ((add16temp & 0x10000) > 0 ? F_CARRY : 0) | overflow_add[lookup >> 4] | (H & (F_3 | F_5 | F_SIGN)) | halfcarry_add[lookup & 0x07] | (HL > 0 ? 0 : F_ZERO);
        }

        //8 bit subtract to accumulator (no carry)
        public void Sub_R(int reg) {
            int subtemp = A - reg;
            byte lookup = (byte)(((A & 0x88) >> 3) | ((reg & 0x88) >> 2) | ((subtemp & 0x88) >> 1));
            A = subtemp & 0xff;
            F = ((subtemp & 0x100) > 0 ? F_CARRY : 0) | F_NEG | halfcarry_sub[lookup & 0x07] | overflow_sub[lookup >> 4] | sz53[A];
        }

        //8 bit subtract from accumulator with carry (SBC A, r)
        public void Sbc_R(int reg) {
            int sbctemp = A - reg - (F & F_CARRY);
            byte lookup = (byte)(((A & 0x88) >> 3) | ((reg & 0x88) >> 2) | ((sbctemp & 0x88) >> 1));
            A = sbctemp & 0xff;
            F = ((sbctemp & 0x100) > 0 ? F_CARRY : 0) | F_NEG | halfcarry_sub[lookup & 0x07] | overflow_sub[lookup >> 4] | sz53[A];
        }

        //16 bit subtract from HL with carry
        public void Sbc_RR(int reg) {
            int sub16temp = HL - reg - (F & F_CARRY);
            byte lookup = (byte)(((HL & 0x8800) >> 11) | ((reg & 0x8800) >> 10) | ((sub16temp & 0x8800) >> 9));
            HL = sub16temp & 0xffff;
            F = ((sub16temp & 0x10000) > 0 ? F_CARRY : 0) | F_NEG | overflow_sub[lookup >> 4] | (H & (F_3 | F_5 | F_SIGN)) | halfcarry_sub[lookup & 0x07] | (HL > 0 ? 0 : F_ZERO);
        }

        //Comparison with accumulator
        public void Cp_R(int reg) {
            int cptemp = A - reg;
            byte lookup = (byte)(((A & 0x88) >> 3) | ((reg & 0x88) >> 2) | ((cptemp & 0x88) >> 1));
            F = ((cptemp & 0x100) > 0 ? F_CARRY : (cptemp > 0 ? 0 : F_ZERO)) | F_NEG | halfcarry_sub[lookup & 0x07] | overflow_sub[lookup >> 4] | (reg & (F_3 | F_5)) | (cptemp & F_SIGN);
        }

        //AND with accumulator
        public void And_R(int reg) {
            A &= reg;
            F = F_HALF | sz53p[A];
            if ((reg & ~96) == 0 && reg != 96)
                and_32_Or_64 = true;
        }

        //XOR with accumulator
        public void Xor_R(int reg) {
            A = (A ^ reg) & 0xff;
            F = sz53p[A];
        }

        //OR with accumulator
        public void Or_R(int reg) {
            A |= reg;
            F = sz53p[A];
        }

        //Rotate left with carry register (RLC r)
        public int Rlc_R(int reg) {
            reg = ((reg << 1) | (reg >> 7)) & 0xff;
            F = (reg & F_CARRY) | sz53p[reg];
            return reg;
        }

        //Rotate right with carry register (RLC r)
        public int Rrc_R(int reg) {
            F = reg & F_CARRY;
            reg = ((reg >> 1) | (reg << 7)) & 0xff;
            F |= sz53p[reg];
            return reg;
        }

        //Rotate left register (RL r)
        public int Rl_R(int reg) {
            byte rltemp = (byte)(reg & 0xff);
            reg = ((reg << 1) | (F & F_CARRY)) & 0xff;
            F = (rltemp >> 7) | sz53p[reg];
            return reg;
        }

        //Rotate right register (RL r)
        public int Rr_R(int reg) {
            byte rrtemp = (byte)(reg & 0xff);
            reg = ((reg >> 1) | (F << 7)) & 0xff;
            F = (rrtemp & F_CARRY) | sz53p[reg];
            return reg;
        }

        //Shift left arithmetic register (SLA r)
        public int Sla_R(int reg) {
            F = reg >> 7;
            reg = (reg << 1) & 0xff;
            F |= sz53p[reg];
            return reg;
        }

        //Shift right arithmetic register (SRA r)
        public int Sra_R(int reg) {
            F = reg & F_CARRY;
            reg = ((reg & 0x80) | (reg >> 1)) & 0xff;
            F |= sz53p[reg];
            return reg;
        }

        //Shift left logical register (SLL r)
        public int Sll_R(int reg) {
            F = reg >> 7;
            reg = ((reg << 1) | 0x01) & 0xff;
            F |= sz53p[reg];
            return reg;
        }

        //Shift right logical register (SRL r)
        public int Srl_R(int reg) {
            F = reg & F_CARRY;
            reg = (reg >> 1) & 0xff;
            F |= sz53p[reg];
            return reg;
        }

        //Bit test operation (BIT b, r)
        public void Bit_R(int b, int reg) {
            F = (F & F_CARRY) | F_HALF | (reg & (F_3 | F_5));
            if (!((reg & (0x01 << b)) > 0)) F |= F_PARITY | F_ZERO;
            if (b == 7 && (reg & 0x80) > 0) F |= F_SIGN;
        }

        //Reset bit operation (RES b, r)
        public int Res_R(int b, int reg) {
            reg = reg & ~(1 << b);
            return reg;
        }

        //Set bit operation (SET b, r)
        public int Set_R(int b, int reg) {
            reg = reg | (1 << b);
            return reg;
        }

        //Decimal Adjust Accumulator (DAA)
        public void DAA() {
            int ans = A;
            int incr = 0;
            bool carry = (F & F_CARRY) != 0;

            if ((F & F_HALF) != 0 || (ans & 0x0f) > 0x09) {
                incr |= 0x06;
            }

            if (carry || ans > 0x9f || ans > 0x8f && (ans & 0x0f) > 0x09) {
                incr |= 0x60;
            }

            if (ans > 0x99) {
                carry = true;
            }

            if ((F & F_NEG) != 0) {
                Sub_R(incr);
            }
            else {
                Add_R(incr);
            }

            ans = A;

            SetCarry(carry);
            SetParity(parity[ans]);
        }

        //Returns parity of a number (true if there are even numbers of 1, false otherwise)
        //Superseded by the table method.
        public bool GetParity(int val) {
            bool parity = false;
            int runningCounter = 0;
            for (int count = 0; count < 8; count++) {
                if ((val & 0x80) != 0)
                    runningCounter++;
                val = val << 1;
            }

            if (runningCounter % 2 == 0)
                parity = true;

            return parity;
        }

        public int GetDisplacement(int val) {
            int res = (128 ^ val) - 128;
            return res;
        }
    }
}