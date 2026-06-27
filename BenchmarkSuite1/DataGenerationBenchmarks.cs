using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using SearchComparisonNet.Kernel.Models;
using Microsoft.VSDiagnostics;

namespace SearchComparisonNet.Benchmarks;
[SimpleJob(warmupCount: 3, iterationCount: 5)]
[CPUUsageDiagnoser]
public class DataGenerationBenchmarks
{
    private DataGenerator _generator = null!;
    // Typical run uses 500_000 entries; smaller sizes show how generation scales.
    [Params(10_000, 100_000, 500_000)]
    public int NoOfEntries { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Constructing the generator eagerly produces one dataset (unmeasured);
        // the benchmark re-invokes GenerateData() to measure a fresh generation each op.
        _generator = new DataGenerator(new DataParameters(NoOfEntries));
    }

    [Benchmark]
    public ObservableCollection<int> GenerateData() => _generator.GenerateData();
}