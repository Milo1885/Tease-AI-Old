''' <summary>
''' Interface for media access
''' </summary>
Public Interface IMediaAccess
    ''' <summary>
    ''' Adds a new media folder to the repository
    ''' </summary>
    ''' <param name="id">Id that is used for retrieval with <see cref="Folders"/></param>
    ''' <param name="folder">Instance of <see cref="MediaFolder"/>.</param>
    Sub AddMediaFolder(ByVal id As Guid, ByVal folder As MediaFolder)
    ''' <summary>
    ''' Gives access to stored MediaFolders.
    ''' </summary>
    ''' <param name="index">Id of the folder to retrieve that was given when <see cref="AddMediaFolder(Guid, MediaFolder)"/> was called.</param>
    ReadOnly Property Folders(ByVal index As Guid) As MediaFolder
End Interface
