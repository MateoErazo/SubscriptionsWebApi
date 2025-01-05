DECLARE @startDate datetime = '2025-01-01';
DECLARE @endDate datetime = '2025-02-01';
DECLARE @amountPerRequest decimal(4,4) = 1.0/2 --2.0/1000; --2 dollars per every 1000 requests

INSERT INTO Invoices (UserId, Amount, IssueDate, Paid, DueDate)
SELECT
ak.UserId,
--COUNT(*) AS CountRegs,
COUNT(*) * @amountPerRequest AS Amount,
GETDATE() AS IssuedDate,
0 AS Paid,
DATEADD(d,60,GETDATE()) AS DueDate

FROM Requests req
INNER JOIN APIKeys ak ON ak.Id = req.APIKeyId
WHERE ak.KeyType != 1 AND req.RequestDate >= @startDate AND req.RequestDate < @endDate
GROUP BY ak.UserId

INSERT INTO IssuedInvoices ([Month], [Year])
SELECT
	CASE MONTH (GETDATE())
		WHEN 1 THEN 12
		ELSE MONTH(GETDATE()) -1 END AS [Month],
	CASE MONTH (GETDATE())
		WHEN 1 THEN YEAR(GETDATE()) -1
		ELSE YEAR(GETDATE()) END AS [Year]