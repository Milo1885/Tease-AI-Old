Imports System.IO

''' <summary>
''' Represents a folder in the local filesystem containing media files.
''' The folder includes all sub directories.
''' </summary>
Public Class MediaFolder

    Private _directory As Func(Of String)
    Private _isEnabled As Func(Of Boolean)
    Private _fileMasks As String()

    ''' <summary>
    ''' File masks including common video files
    ''' </summary>
    Public Shared ReadOnly Videos() As String = {"*.wmv", "*.avi", "*.mp4", "*.m4v", "*.mpg", "*.mov"}

    ''' <summary>
    ''' Creates a new instance of <see cref="MediaFolder"/>
    ''' </summary>
    ''' <param name="directory">Function that gives the root directory of this mediafolder. This is read everytime the folder is read so changes are reflected without recreation of <see cref="MediaFolder"/>.</param>
    ''' <param name="isEnabled">Function that determines if this media folder is enabled. This is read everytime the folder is read so changes are reflected without recreation of <see cref="MediaFolder"/>.</param>
    ''' <param name="filemasks">An array of filemask strings that determine which files are included.</param>
    Sub New(ByVal directory As Func(Of String), ByVal isEnabled As Func(Of Boolean), ByVal filemasks As String())
        _directory = directory
        _isEnabled = isEnabled
        _fileMasks = filemasks
    End Sub

    ''' <summary>
    ''' Returns the files contained in the folder that
    ''' match the file mask if the media folder is enabled
    ''' </summary>
    ReadOnly Iterator Property Files() As IEnumerable(Of String)
        Get
            If _isEnabled() And Directory.Exists(_directory()) Then
                For Each mask In _fileMasks
                    For Each file In Directory.GetFiles(_directory(), mask, SearchOption.AllDirectories)
                        Yield file
                    Next
                Next
            End If
        End Get
    End Property

End Class
