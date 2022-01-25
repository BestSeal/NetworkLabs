using System.Collections.ObjectModel;

namespace FtpViewer.Views;

public class FileView
{
    //private static 
    
    public string FileName { get; }
    
    public bool IsDir { get; }
    
    public int Size { get; }

    public ObservableCollection<FileView> FileViews { get; set; }

    public FileView(string fileName/*, bool isDir, int size*/)
    {
        FileName = fileName;
        /*IsDir = isDir;
        Size = size;*/
        FileViews = new ObservableCollection<FileView>();
    }
}