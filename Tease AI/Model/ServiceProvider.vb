
Public Class ServiceProvider
    Implements IServiceProvider

    Private Shared ReadOnly _serviceProvider As IServiceProvider = New ServiceProvider
    Private ReadOnly _services As Dictionary(Of Type, Object) = New Dictionary(Of Type, Object)

    ''' <summary>
    ''' Returns default instance of <see cref="IServiceProvider"/>
    ''' </summary>
    Public Shared ReadOnly Property Instance() As IServiceProvider
        Get
            Return _serviceProvider
        End Get
    End Property

    ''' <summary>
    ''' Publishes a service
    ''' </summary>
    Public Sub PublishService(Of t)(serviceImpl As t) Implements IServiceProvider.PublishService
        _services(GetType(t)) = serviceImpl
    End Sub

    ''' <summary>
    ''' Gets an instance of a service of type <typeparamref name="t"/>. If no
    ''' service of the given type was published this method throws an exception.
    ''' </summary>
    ''' <typeparam name="t">Type of service requested</typeparam>
    Public Function GetService(Of t)() As t Implements IServiceProvider.GetService
        Return _services(GetType(t))
    End Function
End Class
