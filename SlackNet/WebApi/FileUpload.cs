#nullable enable
using System;
using System.IO;
using System.Net.Http;

namespace SlackNet.WebApi;

/// <summary>
/// Represents a file to be uploaded, along with some optional metadata.
/// </summary>
public class FileUpload
{
    private readonly Func<FileContent> _getFileContent;

    private FileUpload(string fileName, Func<FileContent> getFileContent)
    {
        _getFileContent = getFileContent;
        FileName = fileName;
    }

    /// <remarks>Caller is responsible for disposing <paramref name="fileContent"/> after the file has been uploaded.</remarks>
    public FileUpload(string fileName, HttpContent fileContent)
        : this(fileName, () => new FileContent(fileContent, true)) { }

    /// <remarks>Caller is responsible for disposing <paramref name="fileContent"/> after the file has been uploaded.</remarks>
    public FileUpload(string fileName, Stream fileContent)
        : this(fileName, () => new FileContent(new StreamContent(fileContent), true)) { }
    
    public FileUpload(string fileName, byte[] fileContent)
        : this(fileName, () => new FileContent(new ByteArrayContent(fileContent))) { }
    
    public FileUpload(string fileName, string fileContent)
        : this(fileName, () => new FileContent(new StringContent(fileContent))) { }

    public FileUpload(string localFilePath)
        : this(Path.GetFileName(localFilePath), () => new FileContent(new StreamContent(System.IO.File.OpenRead(localFilePath)))) { }

    /// <summary>
    /// Name of the file being uploaded.
    /// </summary>
    public string FileName { get; }
    
    /// <summary>
    /// Optional title for the file.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Description of image for screen-reader.
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// Syntax type of the snippet being uploaded.
    /// </summary>
    public string? SnippetType { get; set; }
    
    internal FileContent GetContent() => _getFileContent();
}

class FileContent(HttpContent httpContent, bool externallyOwned = false) : IDisposable
{
    public HttpContent HttpContent => httpContent;
    
    public void Dispose()
    {
        if (!externallyOwned)
            httpContent.Dispose();
    }
} 