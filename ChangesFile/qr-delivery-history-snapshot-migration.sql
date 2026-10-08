/*
  QR Delivery immutable history migration

  Deployment order:
    1. Back up tblservicingqrdetail and the two affected stored procedures.
    2. Run Section A on the target database.
    3. Add the versioned INSERT branch from Section B to spInsertCollectionDetail.
    4. Replace the QR Delivery History branch in spViewAdvanceDetail using
       ChangesFile/db-dump.sql (search for "QR Delivery History").
    5. Test both an old MIS build and the new MIS build before publishing.

  Do not include tblservicingqrdetail in ordinary IR/service test-data cleanup.
*/

/* Section A: schema change. Run once. */
ALTER TABLE tblservicingqrdetail
    ADD COLUMN MerchantName varchar(255) NOT NULL DEFAULT '' AFTER QRResult,
    ADD COLUMN MerchantAddress text NULL AFTER MerchantName,
    ADD COLUMN TID varchar(100) NOT NULL DEFAULT '' AFTER MerchantAddress,
    ADD COLUMN MID varchar(100) NOT NULL DEFAULT '' AFTER TID,
    ADD COLUMN TerminalSN varchar(100) NOT NULL DEFAULT '' AFTER MID,
    ADD COLUMN SIMSN varchar(100) NOT NULL DEFAULT '' AFTER TerminalSN,
    ADD COLUMN JobType int NOT NULL DEFAULT 0 AFTER SIMSN;

/*
  Backfill legacy rows while the referenced IR, merchant, and service rows still
  exist. JSON fallbacks recover values from previously saved QR payloads when
  the live row has already been removed. JSON_VALID prevents malformed legacy
  payloads from aborting the migration.
*/
UPDATE tblservicingqrdetail q
LEFT JOIN tblirdetail ir
    ON q.IRIDNo = ir.IRIDNo
LEFT JOIN tblparticular mer
    ON q.MerchantID = mer.ParticularID
LEFT JOIN tblservicingdetail svc
    ON q.ServiceNo = svc.ServiceNo
SET
    q.MerchantName = COALESCE(
        NULLIF(q.MerchantName, ''),
        NULLIF(mer.Name, ''),
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.MerchantName')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.merchantName')) END,
        ''),
    q.MerchantAddress = COALESCE(
        NULLIF(q.MerchantAddress, ''),
        NULLIF(mer.Address, ''),
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.MerchantAddress')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            COALESCE(
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.merchantAddress')),
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.address')))
        END,
        ''),
    q.TID = COALESCE(
        NULLIF(q.TID, ''),
        NULLIF(ir.TID, ''),
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.TID')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.tid')) END,
        ''),
    q.MID = COALESCE(
        NULLIF(q.MID, ''),
        NULLIF(ir.MID, ''),
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.MID')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.mid')) END,
        ''),
    q.TerminalSN = COALESCE(
        NULLIF(q.TerminalSN, ''),
        CASE WHEN svc.JobType = 7 THEN NULLIF(svc.ReplaceTerminalSN, '')
             ELSE NULLIF(svc.TerminalSN, '') END,
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.TerminalSN')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            COALESCE(
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.terminalSN')),
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.terminalSerialNo')))
        END,
        ''),
    q.SIMSN = COALESCE(
        NULLIF(q.SIMSN, ''),
        CASE WHEN svc.JobType = 7 THEN NULLIF(svc.ReplaceSIMSN, '')
             ELSE NULLIF(svc.SIMSerialNo, '') END,
        CASE WHEN JSON_VALID(q.InternalQRContent) THEN
            JSON_UNQUOTE(JSON_EXTRACT(q.InternalQRContent, '$.SIMSN')) END,
        CASE WHEN JSON_VALID(q.QRContent) THEN
            COALESCE(
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.simSN')),
                JSON_UNQUOTE(JSON_EXTRACT(q.QRContent, '$.simSerialNo')))
        END,
        ''),
    q.JobType = CASE WHEN q.JobType = 0 THEN COALESCE(svc.JobType, 0)
                     ELSE q.JobType END;

/* Section B: add this ELSEIF before the final ELSE/END IF of
   spInsertCollectionDetail. Retain the old 'QR Delivery Detail' branch. */
/*
ELSEIF (p_MaintenanceType = 'QR Delivery Detail Snapshot')
THEN
    SET @QUERY = CONCAT(
        'INSERT INTO tblservicingqrdetail(',
        'ServiceNo, IRIDNo, MerchantID, QRDate, QRContent, InternalQRContent, ',
        'ProcessedBy, InventoryStatus, TerminalPrepStatus, DispatcherStatus, ',
        'QRResult, MerchantName, MerchantAddress, TID, MID, TerminalSN, SIMSN, JobType',
        ') VALUES ', p_SQL);
*/

/* Verification after deploying the stored-procedure changes. */
SELECT
    QRID,
    ServiceNo,
    IRIDNo,
    MerchantID,
    MerchantName,
    TID,
    MID,
    TerminalSN,
    SIMSN,
    JobType,
    QRResult,
    DateTimeStamp
FROM tblservicingqrdetail
ORDER BY QRID DESC
LIMIT 20;
