using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core.Warnings
{
    public class WarningDiscardFailuresPreprocessor : IFailuresPreprocessor
    {
        private readonly ILogger _logger;

        private WarningDiscardFailuresPreprocessor(ILogger logger = null)
        {
            if (logger != null)
                _logger = logger;
        }

        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            var failures = failuresAccessor.GetFailureMessages();

            foreach (var failure in failures)
            {
                //FailureDefinitionId id = failure.GetFailureDefinitionId();
                var failureSeverity = failuresAccessor.GetSeverity();

                //Simply eat all warnings
                if (failureSeverity == FailureSeverity.Warning)
                {
                    _logger?.Warn($"'{failure.GetDescriptionText()}'");
                    failuresAccessor.DeleteWarning(failure);
                }
            }

            return FailureProcessingResult.Continue;
        }

        public static Transaction GetTransaction(Document doc, ILogger logger = null)
        {
            var transaction = new Transaction(doc);
            var failureHandlingOptions = transaction.GetFailureHandlingOptions();

            failureHandlingOptions.SetFailuresPreprocessor(new WarningDiscardFailuresPreprocessor(logger));
            transaction.SetFailureHandlingOptions(failureHandlingOptions);

            return transaction;
        }
    }
}