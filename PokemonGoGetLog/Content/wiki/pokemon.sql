USE [aspnet-PokemonGoGetLog-20160810085216]
GO

/****** オブジェクト: Table [dbo].[PokemonDatas] スクリプト日付: 2016/08/13 15:40:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PokemonDatas] (
    [Id]     INT           NOT NULL,
    [JpName] NVARCHAR (50) NOT NULL,
    [EnName] NVARCHAR (50) NOT NULL
);


