Namespace Models

    ''' <summary>
    ''' 錯誤頁 ViewModel，供 Error.cshtml 使用。
    ''' 對應 Core（C#）的 ErrorViewModel.cs。
    ''' </summary>
    Public Class ErrorViewModel

        Public Property RequestId As String

        ''' <summary>
        ''' 當 RequestId 不為空白時，才在錯誤頁顯示它。
        ''' </summary>
        Public ReadOnly Property ShowRequestId As Boolean
            Get
                Return Not String.IsNullOrEmpty(RequestId)
            End Get
        End Property

    End Class

End Namespace
