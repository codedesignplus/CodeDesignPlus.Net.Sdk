namespace CodeDesignPlus.Net.File.Storage.Test.Helpers;

public static class Extensions
{
    public static bool CompareStreams(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * sizeof(long);
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            int count1 = stream1.Read(buffer1, 0, bufferSize);
            int count2 = stream2.Read(buffer2, 0, bufferSize);

            if (count1 != count2)
                return false;

            if (count1 == 0)
                return true;

            if (!buffer1.SequenceEqual(buffer2))
                return false;
        }
    }
}
