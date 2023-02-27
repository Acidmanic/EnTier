

-- EventStream Example.EventSourcing.Meadow.EventSourcing.IPostEvent
-- ---------------------------------------------------------------------------------------------------------------------
-- EventStreamSqlScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE PostsEventStream (
    EventId bigint PRIMARY KEY IDENTITY (1,1),
    StreamId bigint,
    TypeName nvarchar(256),
    SerializedValue nvarchar(1024));
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertPostEvent(
                                   @StreamId bigint,
                                   @TypeName nvarchar(256),
                                   @SerializedValue nvarchar(1024)) AS

    INSERT INTO PostsEventStream (StreamId, TypeName, SerializedValue) 
        VALUES (@StreamId,@TypeName,@SerializedValue);
    
    DECLARE @NewId bigint = (IDENT_CURRENT('PostsEventStream'));
SELECT * FROM PostsEventStream WHERE EventId=@NewId;
    
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPostStreams AS
    SELECT * FROM PostsEventStream;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPostStreamByStreamId(@StreamId bigint) AS

    SELECT * FROM PostsEventStream WHERE StreamId = @StreamId;

GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPostStreamsChunk(@BaseEventId bigint, @Count bigint) AS

    SELECT TOP (@Count) * FROM PostsEventStream WHERE EventId > @BaseEventId
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPostStreamChunkByStreamId(
                                        @StreamId bigint,
                                        @BaseEventId bigint,
                                        @Count bigint) AS

    SELECT TOP (@Count) * FROM PostsEventStream WHERE StreamId = @StreamId AND EventId > @BaseEventId

GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </EventStream>
-- ---------------------------------------------------------------------------------------------------------------------

