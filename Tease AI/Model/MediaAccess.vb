Imports Tease_AI

Public Class MediaAccess
    Implements IMediaAccess

    Private ReadOnly _mediaFolders As Dictionary(Of Guid, MediaFolder) = New Dictionary(Of Guid, MediaFolder)

    Public ReadOnly Property Folders(index As Guid) As MediaFolder Implements IMediaAccess.Folders
        Get
            Return _mediaFolders(index)
        End Get
    End Property

    Public Sub AddMediaFolder(id As Guid, folder As MediaFolder) Implements IMediaAccess.AddMediaFolder
        _mediaFolders(id) = folder
    End Sub
End Class
