 SELECT T.Id AS timeId, 
    U.Id AS userId, 
   P.Id AS projId,
   O.Id AS orgId,
   T.clockInTime AS inTime,
  T.clockOutTime AS outTime,
    O.name AS orgName,
   P.name AS projName, 
  ISNULL(U.firstName,U.emailAddress) + ' ' + ISNULL(U.lastName,U.emailAddress) AS volName ,
  CONVERT(DECIMAL(12,2),DATEDIFF(n,T.clockInTime,T.clockOutTime) / 60.0) AS elapsedHrs 
  FROM dbo.TimeSheet T LEFT JOIN dbo.[User] U ON T.user_Id = U.Id 
   LEFT JOIN dbo.Organization O ON T.org_Id = O.Id 
   LEFT JOIN dbo.Project P ON P.Id = T.project_Id
   where t.Id < 20
   AND T.clockInTime > CONVERT(DATE, '11/23/2015')
 ORDER BY T.clockInTime DESC 