﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using static System.Diagnostics.Debug;

namespace Microsoft.VisualStudio.ProjectSystem.SpecialFileProviders
{
    [ExportSpecialFileProvider(SpecialFiles.AppConfig)]
    [AppliesTo(ProjectCapability.CSharpOrVisualBasic)]
    internal class AppConfigFileSpecialFileProvider : AbstractSpecialFileProvider
    {
        [ImportingConstructor]
        public AppConfigFileSpecialFileProvider([Import(ExportContractNames.ProjectTreeProviders.PhysicalProjectTreeService)] IProjectTreeService projectTreeService,
                                                [Import(ExportContractNames.ProjectItemProviders.SourceFiles)] IProjectItemProvider sourceItemsProvider,
                                                [Import(AllowDefault = true)] Lazy<ICreateFileFromTemplateService> templateFileCreationService,
                                                IFileSystem fileSystem) 
            : base(projectTreeService, sourceItemsProvider, templateFileCreationService, fileSystem)
        {
        }

        protected override string GetFileNameOfSpecialFile(SpecialFiles fileId)
        {
            Assert(fileId == SpecialFiles.AppConfig);
            return "App.config";
        }

        protected override string GetTemplateForSpecialFile(SpecialFiles fileId)
        {
            Assert(fileId == SpecialFiles.AppConfig);
            return "AppConfigurationInternal.zip";
        }

        protected override bool CreatedByDefaultUnderAppDesignerFolder => false;
    }
}
