public static class ConvertNumber
{
    public static string ConvertNumberToLetteredString(float num)
    {
        if (num > 1000000000000)
        {
            return "" + (num / 1000000000000).ToString("n2") + "T";
        }

        if (num > 1000000000)
        {
            return "" + (num / 1000000000).ToString("n2") + "B";
        }

        if (num > 1000000)
        {
            return "" + (num / 1000000).ToString("n2") + "M";
        }
        if (num > 1000)
        {
            return "" + (num / 1000).ToString("n2") + "K";
        }

        return "" + num.ToString("n0");
    }
}