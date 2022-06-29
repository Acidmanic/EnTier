------------------------------------------------------------------------------------------------------------------------
--                                     Table
------------------------------------------------------------------------------------------------------------------------
create table Posts
(
    Id      bigint primary key identity (1,1),
    Title   nvarchar(32),
    Content nvarchar(256)
)
------------------------------------------------------------------------------------------------------------------------
--SPLIT
------------------------------------------------------------------------------------------------------------------------
insert into Posts (Title, Content)
values ('First Builtin Post', 'This post is created by application to indicate its up and running')
------------------------------------------------------------------------------------------------------------------------
--SPLIT
------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
--                                      Procedures
------------------------------------------------------------------------------------------------------------------------
create procedure spReadAllPosts
as
    select * from Posts
go
------------------------------------------------------------------------------------------------------------------------
create procedure spReadPostById(@Id bigint)
as
    select * from Posts
    where Id = @Id
go
------------------------------------------------------------------------------------------------------------------------
create procedure spInsertPost(@Title nvarchar(32),@Content nvarchar(256))
as

    insert into Posts (Title, Content)
    values (@Title, @Content)
    DECLARE @newId bigint=(IDENT_CURRENT('Posts'));
    SELECT * FROM Posts WHERE Id=@newId;
go
------------------------------------------------------------------------------------------------------------------------
CREATE  PROCEDURE spDeletePostById(@Id bigint)
AS
    DECLARE @existing int = (SELECT COUNT(*) FROM Posts);
    DELETE FROM Posts WHERE Id=@Id
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM Posts);
        IF @delta > 0 or @existing = 0
            SELECT cast(1 as bit) Success
        ELSE
            select cast(0 as bit) Success
GO
------------------------------------------------------------------------------------------------------------------------