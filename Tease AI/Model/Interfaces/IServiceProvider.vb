Public Interface IServiceProvider
    Function GetService(Of t)() As t

    Sub PublishService(Of t)(ByVal serviceImpl As t)
End Interface
