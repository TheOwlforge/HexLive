/**
 * https://www.chadgolden.com/blog/finding-all-the-permutations-of-an-array-in-c-sharp
 */

using System;
using System.Collections.Generic;

public class Permutation
{
    public static List<List<int>> Permute(int[] nums)
    {
        var list = new List<List<int>>();
        return DoPermute(nums, 0, nums.Length - 1, list);
    }

    private static List<List<int>> DoPermute(int[] nums, int start, int end, List<List<int>> list)
    {
        if (start == end)
        {
            // We have one of our possible n! solutions,
            // add it to the list.
            list.Add(new List<int>(nums));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                Swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                Swap(ref nums[start], ref nums[i]);
            }
        }

        return list;
    }

    private static void Swap(ref int a, ref int b)
    {
        var temp = a;
        a = b;
        b = temp;
    }
}
