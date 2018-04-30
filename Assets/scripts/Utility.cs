//A static class that will hold a variety of methods that are overall useful for the entire project

using System.Collections;
using System.Collections.Generic;

public static class Utility {

    //The Fisher Yates shuffle implementation google for more info
    public static T[] shuffleArray<T>(T[] array, int seed)
    {
        System.Random rng = new System.Random(seed);

        for(int i = 0; i < array.Length -1; i++)
        {
            int randomIndex = rng.Next(i, array.Length);
            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }

        return array;
    }


}
