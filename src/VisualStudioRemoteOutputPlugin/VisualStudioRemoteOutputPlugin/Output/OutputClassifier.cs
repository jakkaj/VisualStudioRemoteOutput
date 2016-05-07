//Based on https://github.com/mike-ward/VSColorOutput/blob/master/VSColorOutput/Output/ColorClassifier/OutputClassifier.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using VisualStudioRemoteOutputPlugin.Util;

// ReSharper disable EmptyGeneralCatchClause
#pragma warning disable 67

namespace VisualStudioRemoteOutputPlugin.Output
{
    public class OutputClassifier : IClassifier
    {
        private int _initialized;
       
        private IClassificationTypeRegistryService _classificationTypeRegistry;
        private IClassificationFormatMapService _formatMapService;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public void Initialize(IClassificationTypeRegistryService registry, IClassificationFormatMapService formatMapService)
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 1) return;
            try
            {
                _classificationTypeRegistry = registry;
                _formatMapService = formatMapService;

                //Settings.SettingsUpdated += (sender, args) =>
                //{
                //    UpdateClassifiers();
                //    UpdateFormatMap();
                //};
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            try
            {
                var spans = new List<ClassificationSpan>();
                var snapshot = span.Snapshot;
                if (snapshot == null || snapshot.Length == 0) return spans;
                

                var start = span.Start.GetContainingLine().LineNumber;
                var end = (span.End - 1).GetContainingLine().LineNumber;
                for (var i = start; i <= end; i++)
                {
                    var line = snapshot.GetLineFromLineNumber(i);
                    if (line == null) continue;
                    var snapshotSpan = new SnapshotSpan(line.Start, line.Length);
                    var text = line.Snapshot.GetText(snapshotSpan);
                    if (string.IsNullOrEmpty(text)) continue;

                    
                }
                return spans;
            }
            catch (RegexMatchTimeoutException)
            {
                // eat it.
                return new List<ClassificationSpan>();
            }
            catch (NullReferenceException)
            {
                // eat it.    
                return new List<ClassificationSpan>();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
        }

    }
}