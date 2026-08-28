using System;

namespace MIS
{
    // Dependency-free checks that can be run from a debugger or test harness while
    // the final terminal QR and persistence contracts are still being approved.
    internal static class QRDeliveryValidationMockTests
    {
        internal static void RunAll()
        {
            QRDeliveryValidator validator = new QRDeliveryValidator();
            QRDeliveryData expected = new QRDeliveryData
            {
                TID = "TID-100",
                MID = "MID-200",
                MerchantName = "Sample Merchant",
                TerminalSerialNo = "TERM-300",
                SimSerialNo = "SIM-400"
            };

            Assert(validator.Validate(
                "{\"tid\":\"TID-100\",\"mid\":\"MID-200\",\"merchantName\":\"Sample Merchant\",\"terminalSerialNo\":\"TERM-300\",\"simSerialNo\":\"SIM-400\"}",
                expected).IsMatch, "Matching values must pass.");

            Assert(!validator.Validate(
                "{\"tid\":\"WRONG\",\"mid\":\"MID-200\",\"merchantName\":\"Sample Merchant\",\"terminalSerialNo\":\"TERM-300\",\"simSerialNo\":\"SIM-400\"}",
                expected).IsMatch, "A mismatched TID must fail.");

            QRDeliveryValidationResult missing = validator.Validate(
                "{\"tid\":\"TID-100\",\"mid\":\"MID-200\"}", expected);
            Assert(!missing.IsMatch && missing.MissingFields.Count == 3,
                "Missing required fields must be reported and fail validation.");

            bool invalidJsonRejected = false;
            try
            {
                validator.Validate("not-json", expected);
            }
            catch (QRDeliveryValidationException)
            {
                invalidJsonRejected = true;
            }
            Assert(invalidJsonRejected, "Malformed JSON must be rejected.");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException("QR Delivery mock test failed: " + message);
        }
    }
}
