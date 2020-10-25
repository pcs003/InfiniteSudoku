using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SudokuEasyData : MonoBehaviour
{
    public static SudokuData.SudokuBoardData getData()
    {
        SudokuData.SudokuBoardData data;

        var rand = new System.Random();
        int randNum = rand.Next(10,30);

        data = new SudokuData.SudokuBoardData(SudokuData.GenerateSudoku(randNum));
    
        return data;
    }
}

public class SudokuMediumData : MonoBehaviour
{
    public static SudokuData.SudokuBoardData getData()
    {
        SudokuData.SudokuBoardData data;

        var rand = new System.Random();
        int randNum = rand.Next(30, 50);

        data = new SudokuData.SudokuBoardData(SudokuData.GenerateSudoku(randNum));

        return data;
    }
}

public class SudokuHardData : MonoBehaviour
{
    public static SudokuData.SudokuBoardData getData()
    {
        SudokuData.SudokuBoardData data;

        var rand = new System.Random();
        int randNum = rand.Next(50, 70);

        data = new SudokuData.SudokuBoardData(SudokuData.GenerateSudoku(randNum));

        return data;
    }
}





public class SudokuData : MonoBehaviour
{
    public static readonly int order = 3; // INPUT ORDER OF DESIRED SUDOKU HERE
    public static readonly int N = order * order;
    private static System.Random rng = new System.Random();

    public static int[,] mat;

    public static SudokuData Instance;

    public static string desiredDifficulty;

    public struct SudokuBoardData
    {
        public int[] unsolvedData;

        public SudokuBoardData(int[] unsolved) : this()
        {
            this.unsolvedData = unsolved;
        }
    };

    public Dictionary<string, List<SudokuBoardData>> sudokuGame = new Dictionary<string, List<SudokuBoardData>>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static int[] GenerateSudoku(int numRemoved)
    {
        mat = new int[N,N];

        //for (int i = 0; i < N; i++)
        //{
        //    List<int> sublist = new List<int>();
        //    for (int j = 0; j < N; j++)
        //    {
        //        sublist.Add(0);
        //    }

        //    mat.Add(sublist);
        //}

        fillArray(mat, 0, 0);

        int numToBeRemoved = numRemoved; // INSERT NUM TO BE REMOVED HERE
        mat = removeRandom(mat, numToBeRemoved);

        int[] output = new int[N * N];
        int idx = 0;

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                output[idx] = mat[i, j];
                idx++;
            }
        }

        return output;
    }

    // returns false if there is a duplicate
    public static bool checkDups(List<int> vec)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        for (int i = 0; i < vec.Count; ++i)
        {
            if (map.ContainsKey(vec[i]))
            {
                return false;
            }
            map.Add(vec[i], vec[i]);
        }
        return true;
    }
    // returns false if there is a duplicate
    public static bool checkRow(int row, int[,] mat)
    {
        List<int> ivec = new List<int>();
        for (int i = 0; i < N; ++i)
        {
            // don't include the 0s
            int val = mat[row, i];
            if (val != 0)
            {
                ivec.Add(val);
            }
        }
        return checkDups(ivec);
    }
    public static bool checkCol(int col, int[,] mat)
    {
        List<int> ivec = new List<int>();
        for (int i = 0; i < N; ++i)
        {
            int val = mat[i, col];
            if (val != 0)
            {
                ivec.Add(val);
            }
        }
        return checkDups(ivec);
    }
    public static bool checkBox(int row, int col, int[,] mat)
    {
        // get top lop left corner coords
        List<int> ivec = new List<int>();
        int trow = (row / order) * order;
        int lcol = (col / order) * order;
        for (int i = 0; i < order; ++i)
        {
            for (int j = 0; j < order; ++j)
            {
                int val = mat[trow + i, lcol + j];
                if (val != 0)
                {
                    ivec.Add(val);
                }
            }
        }
        return checkDups(ivec);
    }

    // Fisher-Yates Shuffle
    public static List<int> Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static bool fillArray(int[,] mat, int row, int col)
    {
        // end condition
        if (row >= N)
        {
            return true;
        }


        // generate array of possible numbers randomly
        List<int> numseq = new List<int>();
        for (int i = 0; i < N; ++i)
        {
            numseq.Add(i + 1);
        }


        numseq = Shuffle(numseq);
        // a while loop that calls next until return value is true
        int index = 0;
        bool itworks = false;


        // while it doesnt work
        while (!itworks)
        {
            // increment to try next value
            index++;
            // if tried all values, 
            if (index == N)
            {
                mat[row, col] = 0;
                return false;
            }
            // fill the current index into the cell

            mat[row, col] = numseq[index];
            // if checkRow is false or checkCol is false
            if (!checkRow(row, mat) || !checkCol(col, mat) || !checkBox(row, col, mat))
            {
                continue;
            }
            // check if it works with current filled in cells
            if (col == N - 1)
            {
                //itworks = checkRow(row, mat) && checkCol(col, mat) && checkBox(row, col, mat) && fillArray(mat, row + 1, 0);
                itworks = fillArray(mat, row + 1, 0);
            }
            else
            {
                //itworks = checkRow(row, mat) && checkCol(col, mat) && checkBox(row, col, mat) && fillArray(mat, row, col + 1);
                itworks = fillArray(mat, row, col + 1);
            }
        }
        // call next 
        return true;
    }


    public static int countDistinct(int[,] mat)
    {
        Dictionary<int, int> set = new Dictionary<int, int>();
        List<int> vec = new List<int>();

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (mat[i,j] != 0)
                {
                    vec.Add(mat[i,j]);
                }
            }
        }

        int count = 0;

        for (int i = 0; i < vec.Count; i++)
        {
            if (!set.ContainsKey(vec[i]))
            {
                set.Add(vec[i], 0);
                count++;
            }
        }

        return count;
    }

    public static int[,] removeRandom(int[,] mat, int numRemoved)
    {
        List<int> numseq = new List<int>(N * N);
        for (int i = 0; i < N * N; ++i)
        {
            numseq.Add(i);
        }

        for (int i = 0; i < numRemoved; i++)
        {
            if (countDistinct(mat) >= N)
            {
                numseq = Shuffle(numseq);

                var rand = new System.Random();
                int randIdx = rand.Next(0, 81);

                int rowIdx = numseq[randIdx] / N;
                int colIdx = numseq[randIdx] % N;

                if (mat[rowIdx, colIdx] == 0)
                {
                    i--;
                }
                else
                {
                    mat[rowIdx, colIdx] = 0;
                }
            }
            else
            {
                break;
            }
        }

        return mat;
    }

    public static int[,] ArrayToMatrix(int[] arr)
    {
        int[,] output = new int[N, N];
        int rowIdx = 0;
        int colIdx = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            rowIdx = i / 9;
            colIdx = i % 9;
            output[rowIdx, colIdx] = arr[i];
        }

        return output;
    }

    public static bool IsCompleted(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == 0)
            {
                return false;
            }
        }

        int[,] mat = ArrayToMatrix(arr);

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (!checkRow(i, mat) || !checkCol(j, mat) || !checkBox(i, j, mat))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
