using System.Collections;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Helpers;

public class CacheTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { Guid.NewGuid() };
        yield return new object[] { "Sample String" };
        yield return new object[] { 123 };
        yield return new object[] { 3.14 };
        yield return new object[] { true };
        yield return new object[] { DateTime.Now };
        yield return new object[] { 123.45M };
        yield return new object[] { new List<int> { 1, 2, 3 } };
        yield return new object[] { new Dictionary<string, int> { { "a", 1 }, { "b", 2 } } };
        yield return new object[] { new int[] { 4, 5, 6 } };
        yield return new object[] { (byte)10 };
        yield return new object[] { (long)10000000000 };
        yield return new object[] { (float)3.14f };
        yield return new object[] { new System.Uri("https://www.example.com") };

        yield return new object[] { Guid.Empty };
        yield return new object[] { "" };
        yield return new object[] { -456 };
        yield return new object[] { -2.71 };
        yield return new object[] { false };
        yield return new object[] { DateTime.MinValue };
        yield return new object[] { -98.76M };
        yield return new object[] { new List<int>() };
        yield return new object[] { new Dictionary<string, int>() };
        yield return new object[] { Array.Empty<int>() };
        yield return new object[] { (byte)0 };
        yield return new object[] { (long)-10000000000 };
        yield return new object[] { (float)-3.14f };
        yield return new object[] { "null string" };

        yield return new object[] { Guid.Parse("7d3a854a-9056-4f6e-ba48-b3d1d80c29ee") };
        yield return new object[] { "Otro Texto" };
        yield return new object[] { -456 };
        yield return new object[] { -2.71 };
        yield return new object[] { false };
        yield return new object[] { DateTime.Parse("2024-01-15T18:00:00") };
        yield return new object[] { -98.76M };
        yield return new object[] { new List<int> { 4, 5 } };
        yield return new object[] { new Dictionary<string, int> { { "x", 50 } } };
        yield return new object[] { new int[] { 7, 8, 9 } };
        yield return new object[] { (byte)255 };
        yield return new object[] { (long)5000000000 };
        yield return new object[] { (float)1.0f };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}