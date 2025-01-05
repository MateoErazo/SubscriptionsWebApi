UPDATE us
SET NonPayingUser = 1
FROM AspNetUsers us
INNER JOIN Invoices inv ON us.Id = inv.UserId
WHERE inv.Paid = 0 AND inv.DueDate < GETDATE();
