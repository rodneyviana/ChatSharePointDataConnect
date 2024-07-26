CREATE DATABASE [SPGraphConnector]
GO

USE [SPGraphConnector]
GO
/****** Object:  Table [dbo].[GroupMembers]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupMembers](
	[SiteId] [uniqueidentifier] NOT NULL,
	[GroupId] [bigint] NOT NULL,
	[MemberType] [nvarchar](50) NULL,
	[MemberAadObjectId] [uniqueidentifier] NOT NULL,
	[MemberName] [nvarchar](255) NULL,
	[MemberEmail] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC,
	[GroupId] ASC,
	[MemberAadObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Owner]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Owner](
	[SiteId] [uniqueidentifier] NOT NULL,
	[AadObjectId] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC,
	[AadObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RootWeb]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RootWeb](
	[SiteId] [uniqueidentifier] NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](255) NULL,
	[WebTemplate] [nvarchar](50) NULL,
	[WebTemplateId] [int] NULL,
	[LastItemModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecondaryContact]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SecondaryContact](
	[SiteId] [uniqueidentifier] NOT NULL,
	[AadObjectId] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC,
	[AadObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SharedWith]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharedWith](
	[Id] [uniqueidentifier] NOT NULL,
	[SharePointPermissionId] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[EmailAddress] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[SharePointPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SharePointGroups]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharePointGroups](
	[ptenant] [uniqueidentifier] NOT NULL,
	[SiteId] [uniqueidentifier] NOT NULL,
	[GroupId] [bigint] NOT NULL,
	[GroupLinkId] [uniqueidentifier] NULL,
	[GroupType] [nvarchar](50) NULL,
	[DisplayName] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[OwnerAadObjectId] [uniqueidentifier] NULL,
	[OwnerName] [nvarchar](255) NULL,
	[OwnerEmail] [nvarchar](255) NULL,
	[SnapshotDate] [datetime] NULL,
	[Operation] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SharePointPermission]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharePointPermission](
	[Id] [uniqueidentifier] NOT NULL,
	[ptenant] [uniqueidentifier] NOT NULL,
	[SiteId] [uniqueidentifier] NOT NULL,
	[WebId] [uniqueidentifier] NOT NULL,
	[ListId] [uniqueidentifier] NULL,
	[ItemType] [nvarchar](50) NOT NULL,
	[ItemURL] [nvarchar](max) NOT NULL,
	[FileExtension] [nvarchar](10) NULL,
	[RoleDefinition] [nvarchar](50) NOT NULL,
	[LinkId] [uniqueidentifier] NULL,
	[ScopeId] [uniqueidentifier] NOT NULL,
	[LinkScope] [nvarchar](50) NULL,
	[SnapshotDate] [datetime] NOT NULL,
	[ShareCreatedBy] [nvarchar](max) NULL,
	[ShareCreatedTime] [datetime] NULL,
	[ShareLastModifiedBy] [nvarchar](max) NULL,
	[ShareLastModifiedTime] [datetime] NULL,
	[ShareExpirationTime] [datetime] NULL,
	[Operation] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SharePointSites]    Script Date: 7/26/2024 11:23:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharePointSites](
	[ptenant] [uniqueidentifier] NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[Url] [nvarchar](255) NULL,
	[WebCount] [bigint] NULL,
	[StorageQuota] [bigint] NULL,
	[StorageUsed] [bigint] NULL,
	[StorageMetricsMetadataSize] [bigint] NULL,
	[StorageMetricsTotalFileCount] [bigint] NULL,
	[StorageMetricsTotalFileStreamSize] [bigint] NULL,
	[StorageMetricsTotalSize] [bigint] NULL,
	[GroupId] [uniqueidentifier] NULL,
	[GeoLocation] [nvarchar](50) NULL,
	[IsInRecycleBin] [bit] NULL,
	[IsTeamsConnectedSite] [bit] NULL,
	[IsTeamsChannelSite] [bit] NULL,
	[TeamsChannelType] [nvarchar](50) NULL,
	[IsHubSite] [bit] NULL,
	[HubSiteId] [uniqueidentifier] NULL,
	[BlockAccessFromUnmanagedDevices] [bit] NULL,
	[BlockDownloadOfAllFilesOnUnmanagedDevices] [bit] NULL,
	[BlockDownloadOfViewableFilesOnUnmanagedDevices] [bit] NULL,
	[ShareByEmailEnabled] [bit] NULL,
	[ShareByLinkEnabled] [bit] NULL,
	[SensitivityLabelInfoDisplayName] [nvarchar](255) NULL,
	[SensitivityLabelInfoId] [uniqueidentifier] NULL,
	[Classification] [nvarchar](255) NULL,
	[IBMode] [nvarchar](50) NULL,
	[IBSegments] [nvarchar](max) NULL,
	[ReadLocked] [bit] NULL,
	[ReadOnly] [bit] NULL,
	[CreatedTime] [datetime] NULL,
	[LastSecurityModifiedDate] [datetime] NULL,
	[SnapshotDate] [datetime] NULL,
	[Operation] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IDX_GroupMembers_GroupId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_GroupMembers_GroupId] ON [dbo].[GroupMembers]
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_GroupMembers_MemberAadObjectId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_GroupMembers_MemberAadObjectId] ON [dbo].[GroupMembers]
(
	[MemberAadObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_GroupMembers_SiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_GroupMembers_SiteId] ON [dbo].[GroupMembers]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_Owner_SiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_Owner_SiteId] ON [dbo].[Owner]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_RootWeb_SiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_RootWeb_SiteId] ON [dbo].[RootWeb]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SecondaryContact_SiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SecondaryContact_SiteId] ON [dbo].[SecondaryContact]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointGroups_GroupId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointGroups_GroupId] ON [dbo].[SharePointGroups]
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointGroups_ptenant]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointGroups_ptenant] ON [dbo].[SharePointGroups]
(
	[ptenant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointGroups_SiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointGroups_SiteId] ON [dbo].[SharePointGroups]
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointSites_GroupId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointSites_GroupId] ON [dbo].[SharePointSites]
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointSites_HubSiteId]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointSites_HubSiteId] ON [dbo].[SharePointSites]
(
	[HubSiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SharePointSites_ptenant]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointSites_ptenant] ON [dbo].[SharePointSites]
(
	[ptenant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_SharePointSites_Url]    Script Date: 7/26/2024 11:23:27 AM ******/
CREATE NONCLUSTERED INDEX [IDX_SharePointSites_Url] ON [dbo].[SharePointSites]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SharePointGroups] ADD  DEFAULT ('Full') FOR [Operation]
GO
ALTER TABLE [dbo].[SharePointSites] ADD  DEFAULT ('Full') FOR [Operation]
GO
ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD FOREIGN KEY([SiteId], [GroupId])
REFERENCES [dbo].[SharePointGroups] ([SiteId], [GroupId])
GO
ALTER TABLE [dbo].[Owner]  WITH CHECK ADD FOREIGN KEY([SiteId])
REFERENCES [dbo].[SharePointSites] ([Id])
GO
ALTER TABLE [dbo].[RootWeb]  WITH CHECK ADD FOREIGN KEY([SiteId])
REFERENCES [dbo].[SharePointSites] ([Id])
GO
ALTER TABLE [dbo].[SecondaryContact]  WITH CHECK ADD FOREIGN KEY([SiteId])
REFERENCES [dbo].[SharePointSites] ([Id])
GO
ALTER TABLE [dbo].[SharedWith]  WITH CHECK ADD FOREIGN KEY([SharePointPermissionId])
REFERENCES [dbo].[SharePointPermission] ([Id])
GO
ALTER TABLE [dbo].[SharePointGroups]  WITH CHECK ADD FOREIGN KEY([SiteId])
REFERENCES [dbo].[SharePointSites] ([Id])
GO
