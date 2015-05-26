
''' <summary>
''' Core class that manages .. ahem, core stuff :D not sure yet where to go with it
''' 
''' this should initialize the application and hold shared stuff
''' </summary>
Public Class Core

    ''' <summary>
    ''' Initialize the application core, publishing services and so on
    ''' </summary>
    Shared Sub Initialize()
        ServiceProvider.Instance.PublishService(Of IMediaAccess)(New MediaAccess)
    End Sub

End Class
