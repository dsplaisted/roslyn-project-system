﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Option Strict On
Option Explicit On
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Editors.Interop

Namespace Microsoft.VisualStudio.Editors.MyExtensibility
    Friend Class TrackProjectDocumentsEventsHelper
        Implements IVsTrackProjectDocumentsEvents2

        Public Shared Function GetInstance(serviceProvider As IServiceProvider) As TrackProjectDocumentsEventsHelper
            Try
                Return New TrackProjectDocumentsEventsHelper(serviceProvider)
            Catch ex As Exception When Common.Utils.ReportWithoutCrash(ex, "Fail to listen to IVsTrackProjectDocumentsEvents2", NameOf(TrackProjectDocumentsEventsHelper))
                Return Nothing
            End Try
        End Function

        Public Event AfterAddFilesEx(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSADDFILEFLAGS)
        Public Event AfterRemoveFiles(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSREMOVEFILEFLAGS)
        Public Event AfterRenameFiles(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As VSRENAMEFILEFLAGS)
        Public Event AfterRemoveDirectories(cProjects As Integer, cDirectories As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSREMOVEDIRECTORYFLAGS)
        Public Event AfterRenameDirectories(cProjects As Integer, cDirs As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As VSRENAMEDIRECTORYFLAGS)

        ''' ;UnAdviseTrackProjectDocumentsEvents
        ''' <summary>
        ''' Stop listening to IVsTrackProjectDocumentsEvents2
        ''' </summary>
        Public Sub UnAdviseTrackProjectDocumentsEvents()
            If _vsTrackProjectDocumentsEventsCookie <> 0 Then
                If _vsTrackProjectDocuments IsNot Nothing Then
                    _vsTrackProjectDocuments.UnadviseTrackProjectDocumentsEvents(_vsTrackProjectDocumentsEventsCookie)
                    _vsTrackProjectDocuments = Nothing
                End If
                _vsTrackProjectDocumentsEventsCookie = 0
            End If
        End Sub

        Private Sub New(serviceProvider As IServiceProvider)
            If serviceProvider Is Nothing Then
                Throw New ArgumentNullException("serviceProvider")
            End If

            _serviceProvider = serviceProvider

            _vsTrackProjectDocuments = TryCast(_serviceProvider.GetService(GetType(SVsTrackProjectDocuments)), _
                IVsTrackProjectDocuments2)
            If _vsTrackProjectDocuments Is Nothing Then
                Throw New Exception("Could not get IVsTrackProjectDocuments2!")
            End If

            ErrorHandler.ThrowOnFailure( _
                _vsTrackProjectDocuments.AdviseTrackProjectDocumentsEvents(Me, _vsTrackProjectDocumentsEventsCookie))
            Debug.Assert(_vsTrackProjectDocumentsEventsCookie <> 0)
        End Sub

#Region " IVsTrackProjectDocumentsEvents2 methods that are handled "
        Private Function OnAfterAddFilesEx(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSADDFILEFLAGS) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx
            RaiseEvent AfterAddFilesEx(cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments, rgFlags)
            Return NativeMethods.S_OK
        End Function

        Private Function OnAfterRemoveFiles(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSREMOVEFILEFLAGS) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles
            RaiseEvent AfterRemoveFiles(cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments, rgFlags)
            Return NativeMethods.S_OK
        End Function

        Private Function OnAfterRenameFiles(cProjects As Integer, cFiles As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As VSRENAMEFILEFLAGS) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles
            RaiseEvent AfterRenameFiles(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkOldNames, rgszMkNewNames, rgFlags)
            Return NativeMethods.S_OK
        End Function

        Private Function OnAfterRemoveDirectories(cProjects As Integer, cDirectories As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As VSREMOVEDIRECTORYFLAGS) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories
            RaiseEvent AfterRemoveDirectories(cProjects, cDirectories, rgpProjects, rgFirstIndices, rgpszMkDocuments, rgFlags)
            Return NativeMethods.S_OK
        End Function

        Private Function OnAfterRenameDirectories(cProjects As Integer, cDirs As Integer, rgpProjects() As IVsProject, rgFirstIndices() As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As VSRENAMEDIRECTORYFLAGS) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories
            RaiseEvent AfterRenameDirectories(cProjects, cDirs, rgpProjects, rgFirstIndices, rgszMkOldNames, rgszMkNewNames, rgFlags)
            Return NativeMethods.S_OK
        End Function
#End Region

#Region " IVsTrackProjectDocumentsEvents2 methods that are ignored "

        Private Function OnAfterAddDirectoriesEx(cProjects As Integer, cDirectories As Integer, rgpProjects() As Shell.Interop.IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgFlags() As Shell.Interop.VSADDDIRECTORYFLAGS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx
            Return NativeMethods.S_OK
        End Function

        Private Function OnAfterSccStatusChanged(cProjects As Integer, cFiles As Integer, rgpProjects() As Shell.Interop.IVsProject, rgFirstIndices() As Integer, rgpszMkDocuments() As String, rgdwSccStatus() As UInteger) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryAddDirectories(pProject As Shell.Interop.IVsProject, cDirectories As Integer, rgpszMkDocuments() As String, rgFlags() As Shell.Interop.VSQUERYADDDIRECTORYFLAGS, pSummaryResult() As Shell.Interop.VSQUERYADDDIRECTORYRESULTS, rgResults() As Shell.Interop.VSQUERYADDDIRECTORYRESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryAddFiles(pProject As Shell.Interop.IVsProject, cFiles As Integer, rgpszMkDocuments() As String, rgFlags() As Shell.Interop.VSQUERYADDFILEFLAGS, pSummaryResult() As Shell.Interop.VSQUERYADDFILERESULTS, rgResults() As Shell.Interop.VSQUERYADDFILERESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryAddFiles
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryRemoveDirectories(pProject As Shell.Interop.IVsProject, cDirectories As Integer, rgpszMkDocuments() As String, rgFlags() As Shell.Interop.VSQUERYREMOVEDIRECTORYFLAGS, pSummaryResult() As Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS, rgResults() As Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryRemoveFiles(pProject As Shell.Interop.IVsProject, cFiles As Integer, rgpszMkDocuments() As String, rgFlags() As Shell.Interop.VSQUERYREMOVEFILEFLAGS, pSummaryResult() As Shell.Interop.VSQUERYREMOVEFILERESULTS, rgResults() As Shell.Interop.VSQUERYREMOVEFILERESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryRenameDirectories(pProject As Shell.Interop.IVsProject, cDirs As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As Shell.Interop.VSQUERYRENAMEDIRECTORYFLAGS, pSummaryResult() As Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS, rgResults() As Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories
            Return NativeMethods.S_OK
        End Function

        Private Function OnQueryRenameFiles(pProject As Shell.Interop.IVsProject, cFiles As Integer, rgszMkOldNames() As String, rgszMkNewNames() As String, rgFlags() As Shell.Interop.VSQUERYRENAMEFILEFLAGS, pSummaryResult() As Shell.Interop.VSQUERYRENAMEFILERESULTS, rgResults() As Shell.Interop.VSQUERYRENAMEFILERESULTS) As Integer Implements Shell.Interop.IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles
            Return NativeMethods.S_OK
        End Function

#End Region

        Private _serviceProvider As IServiceProvider
        Private _vsTrackProjectDocuments As IVsTrackProjectDocuments2
        Private _vsTrackProjectDocumentsEventsCookie As UInteger
    End Class
End Namespace
