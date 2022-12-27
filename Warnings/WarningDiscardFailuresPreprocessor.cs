using Autodesk.Revit.DB;
using NLog;
using System.Collections.Generic;

namespace ASRR.Revit.Core.Warnings
{
    public class WarningDiscardFailuresPreprocessor : IFailuresPreprocessor
    {
        private readonly ILogger _logger;

        public WarningDiscardFailuresPreprocessor(ILogger logger = null)
        {
            if (logger != null)
                _logger = logger;
        }

        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failures = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor failure in failures)
            {
                //FailureDefinitionId id = failure.GetFailureDefinitionId();
                FailureSeverity failureSeverity = failuresAccessor.GetSeverity();

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
            Transaction transaction = new Transaction(doc);
            FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();

            failureHandlingOptions.SetFailuresPreprocessor(new WarningDiscardFailuresPreprocessor(logger));
            transaction.SetFailureHandlingOptions(failureHandlingOptions);

            return transaction;
        }
    }
}