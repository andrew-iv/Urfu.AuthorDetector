SELECT Count(*)
  FROM [dbo].[BayesClassifierTestSet] where [ClassifierVersion_Id] = 5

  USE [AuthorDetector]
  Go

  Create Function dbo.GetFirstToSecondForClassifier(  @versionId int)
  RETURNS TABLE
AS
RETURN 
(
      SELECT [FirstToSecond]
      ,[MessageCount]
      ,[Success]
  FROM [dbo].[BayesClassifierTestSet] where [ClassifierVersion_Id] = @versionId
);
GO

  Create Function dbo.GetUnsuccessPercentaleForClassifier(  @versionId int, @perc real)
  RETURNS TABLE
AS
RETURN 
Select DISTINCT PERCENTILE_CONT(@perc)
WITHIN GROUP (ORDER BY FirstToSecond) 
                            OVER (PARTITION BY MessageCount,Success) AS MedianCont
							,MessageCount							 
 from dbo.GetFirstToSecondForClassifier(@versionId ) where Success = 0
 Go



 Create Function dbo.GetQualityByFirstToSecondForClassifier(  @versionId int, @perc real)
  RETURNS TABLE
AS
RETURN 
 select Cast(t2.cnt as float)/ t1.cnt as Quality,  T1.MessageCount from (
 Select Count(*) as cnt,MessageCount
 from dbo.GetFirstToSecondForClassifier(@versionId) where Success = 1
 group by MessageCount) T1
 join
 (Select Count(*) as cnt,T1.MessageCount
 from dbo.GetFirstToSecondForClassifier(@versionId) as T1
 join dbo.GetUnsuccessPercentaleForClassifier(@versionId,@perc) as T2 
 on Success = 1 and T1.MessageCount = T2.MessageCount
 where T1.FirstToSecond > T2.MedianCont
 Group by T1.MessageCount) T2
 on T1.MessageCount = T2.MessageCount


   Create Function dbo.GetFirstToAllForClassifier(  @versionId int)
  RETURNS TABLE
AS
RETURN 
(
      SELECT [FirstToAll]
      ,[MessageCount]
      ,[Success]
  FROM [dbo].[BayesClassifierTestSet] where [ClassifierVersion_Id] = @versionId
);
GO

  Create Function dbo.GetUnsuccessFirstToAllPercentaleForClassifier(  @versionId int, @perc real)
  RETURNS TABLE
AS
RETURN 
Select DISTINCT PERCENTILE_CONT(@perc)
WITHIN GROUP (ORDER BY FirstToAll) 
                            OVER (PARTITION BY MessageCount,Success) AS MedianCont
							,MessageCount							 
 from dbo.GetFirstToAllForClassifier(@versionId ) where Success = 0
 Go



 Create Function dbo.GetQualityByFirstToAllForClassifier(  @versionId int, @perc real)
  RETURNS TABLE
AS
RETURN 
 select Cast(t2.cnt as float)/ t1.cnt as Quality,  T1.MessageCount from (
 Select Count(*) as cnt,MessageCount
 from dbo.GetFirstToAllForClassifier(@versionId) where Success = 1
 group by MessageCount) T1
 join
 (Select Count(*) as cnt,T1.MessageCount
 from dbo.GetFirstToAllForClassifier(@versionId) as T1
 join dbo.GetUnsuccessFirstToAllPercentaleForClassifier(@versionId,@perc) as T2 
 on Success = 1 and T1.MessageCount = T2.MessageCount
 where T1.FirstToAll > T2.MedianCont
 Group by T1.MessageCount) T2
 on T1.MessageCount = T2.MessageCount