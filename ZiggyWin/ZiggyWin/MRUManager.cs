using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

public class MRUManager
{
    #region Private members
    private readonly string NameOfProgram;
    private readonly string SubKeyName;
    
    private readonly Action<object, EventArgs> OnRecentFileClick;
    private readonly Action<object, EventArgs> OnClearRecentFilesClick;
    private readonly List<string> fullPath = new List<string>();
    private const int MAX_FILE_PATH_CHARS = 40;

    public class DynamicToolStripMenuItem : ToolStripMenuItem
    {
    }

    private readonly ToolStripMenuItem ParentMenuItem;

    [DllImport("shlwapi.dll")]
    static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

    static string TruncatePath(string path, int length)
    {
        StringBuilder sb = new StringBuilder(length + 1);
        PathCompactPathEx(sb, path, length, 0);
        return sb.ToString();
    }

    private void _onClearRecentFiles_Click(object obj, EventArgs evt)
    {
        try
        {
            RegistryKey rK = Registry.CurrentUser.OpenSubKey(SubKeyName, true);
            if (rK == null)
                return;
            string[] values = rK.GetValueNames();
            foreach (string valueName in values)
                rK.DeleteValue(valueName, true);
            rK.Close();
            ParentMenuItem.DropDownItems.Clear();
            fullPath.Clear();
            ParentMenuItem.Enabled = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        OnClearRecentFilesClick?.Invoke(obj, evt);
    }
        
    private void _refreshRecentFilesMenu()
    {
        RegistryKey rK;
        string s;
        ToolStripItem tSI;
        fullPath.Clear();

        try
        {
            rK = Registry.CurrentUser.OpenSubKey(SubKeyName, false);
            if (rK == null)
            {
                ParentMenuItem.Enabled = false;
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot open recent files registry key:\n" + ex);
            return;
        }

        ParentMenuItem.DropDownItems.Clear();
        string[] valueNames = rK.GetValueNames();
        foreach (string valueName in valueNames.Reverse())
        {
            s = rK.GetValue(valueName, null) as string;
            if (s == null)
                continue;
            fullPath.Add(s);
            tSI = ParentMenuItem.DropDownItems.Add(TruncatePath(s, MAX_FILE_PATH_CHARS));
            tSI.Click += new EventHandler(OnRecentFileClick);
        }

        if (ParentMenuItem.DropDownItems.Count == 0)
        {
            ParentMenuItem.Enabled = false;
            return;
        }
        ParentMenuItem.DropDownItems.Add("-");
        tSI = ParentMenuItem.DropDownItems.Add("Clear list");
        tSI.Click += _onClearRecentFiles_Click;
        ParentMenuItem.Enabled = true;
    }
    #endregion

    #region Public members
    public void AddRecentFile(string fileNameWithFullPath)
    {
        string s;
        try
        {
            RegistryKey rK = Registry.CurrentUser.CreateSubKey(SubKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
            for (int i = 0;; i++)
            {
                s = rK.GetValue(i.ToString(), null) as string;
                if (s == null)
                {
                    rK.SetValue(i.ToString(), fileNameWithFullPath);
                    rK.Close();
                    break;
                }

                if (s == fileNameWithFullPath)
                {
                    rK.Close();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        _refreshRecentFilesMenu();
    }

    public void RemoveRecentFile(string fileNameWithFullPath)
    {
        try
        {
            RegistryKey rK = Registry.CurrentUser.OpenSubKey(SubKeyName, true);
            string[] valuesNames = rK.GetValueNames();
            foreach (string valueName in valuesNames)
            {
                if ((rK.GetValue(valueName, null) as string) == fileNameWithFullPath)
                {
                    rK.DeleteValue(valueName, true);
                    _refreshRecentFilesMenu();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        _refreshRecentFilesMenu();
    }

    public string GetFullFilePath(int index)
    {
        return index < fullPath.Count ? fullPath[index] : null;
    }
    #endregion

    /// <exception cref="ArgumentException">If anything is null or nameOfProgram contains a forward slash or is empty.</exception>
    public MRUManager(ToolStripMenuItem parentMenuItem, string nameOfProgram, Action<object, EventArgs> onRecentFileClick, Action<object, EventArgs> onClearRecentFilesClick = null)
    {
        if(parentMenuItem == null || onRecentFileClick == null ||
            nameOfProgram == null || nameOfProgram.Length == 0 || nameOfProgram.Contains("\\"))
            throw new ArgumentException("Bad argument.");

        ParentMenuItem = parentMenuItem;
        NameOfProgram = nameOfProgram;
        OnRecentFileClick = onRecentFileClick;
        OnClearRecentFilesClick = onClearRecentFilesClick;
        SubKeyName = string.Format("Software\\{0}\\MRU", NameOfProgram);

        _refreshRecentFilesMenu();
    }
}
