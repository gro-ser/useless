using System;
using _;

public class Matrix<T>
{
    static VeryMath<T> op = VeryMath<T>.GetDefault();
    static void SetVeryMath(VeryMath<T> @new)
    {
        if (@new != null) op = @new;
    }

    protected T[,] matrix;
    protected int n, m;
    public int N => n;
    public int M => m;

    public Matrix(T[,] arr)
    {
        matrix = arr;
        n = arr.GetLength(0);
        m = arr.GetLength(1);
    }
    public Matrix(int N, int M)
    {
        matrix = new T[N, M];
        n = N; m = M;
    }
    protected Matrix(int N, int M, T[,] matrix)
    {
        n = N; m = M;
        this.matrix = matrix;
    }

    public T this[int x, int y]
    {
        get { return matrix[x, y]; }
        set { matrix[x, y] = value; }
    }

    public bool Square => n == m;
    public static bool Like(Matrix<T> a, Matrix<T> b) => a.n == b.n && a.m == b.m;

    public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
    {
        if (!Like(a, b)) throw new Exception();
        int n = a.n, m = a.m;
        var tmp = new T[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = op.sum(a.matrix[x, y], b.matrix[x, y]);
        return new Matrix<T>(n, m, tmp);
    }
    public static Matrix<T> operator -(Matrix<T> a, Matrix<T> b)
    {
        if (!Like(a, b)) throw new Exception();
        int n = a.n, m = a.m;
        var tmp = new T[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = op.sub(a.matrix[x, y], b.matrix[x, y]);
        return new Matrix<T>(n, m, tmp);
    }
    public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
    {
        if (a.m != b.n) throw new Exception();
        T[,] tmp = new T[a.n, b.m];
        T sum;
        for (int x = a.n - 1; x >= 0; --x)
        {
            for (int y = b.m - 1; y >= 0; --y)
            {
                sum = default(T);
                for (int i = a.m - 1; i >= 0; --i)
                    sum = op.sum(sum, op.mul(a.matrix[x, i], b.matrix[i, y]));
                tmp[x, y] = sum;
            }
        }
        return new Matrix<T>(tmp);
    }
    public static Matrix<T> operator *(Matrix<T> a, T v)
    {
        int n = a.n, m = a.m;
        var tmp = new T[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = op.mul(a.matrix[x, y], v);
        return new Matrix<T>(n, m, tmp);
    }
    public static Matrix<T> operator /(Matrix<T> a, T v)
    {
        int n = a.n, m = a.m;
        var tmp = new T[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = op.div(a.matrix[x, y], v);
        return new Matrix<T>(n, m, tmp);
    }

    public Matrix<T> AddMinor(int cutX, int cutY)
    {
        var tmp = new T[n - 1, m - 1];
        for (int x = 0, xx = 0; xx < n - 1; x++, xx++)
        {
            if (x == cutX) x++;
            for (int y = 0, yy = 0; yy < m - 1; y++, yy++)
            {
                if (y == cutY) y++;
                tmp[xx, yy] = matrix[x, y];
            }
        }
        return new Matrix<T>(n - 1, m - 1, tmp);
    }
    public T Minor(int x, int y) => AddMinor(x, y).Determinant();
    public T Addition(int x, int y) => op.mul(Minor(x, y), op.convert((((x + y + 1) & 1) * 2 - 1)));
    public T Determinant()
    {
        if (n != m || n <= 0) throw new Exception();
        if (n == 1) return matrix[0, 0];
        if (n == 2) return op.sub(op.mul(matrix[0, 0], matrix[1, 1]), op.mul(matrix[0, 1], matrix[1, 0]));
        if (n == 3)
            return op.sum(op.sum(
            op.sub(op.mul(op.mul(matrix[0, 0], matrix[1, 1]), matrix[2, 2]), op.mul(op.mul(matrix[0, 0], matrix[1, 2]), matrix[2, 1])),
            op.sub(op.mul(op.mul(matrix[0, 1], matrix[1, 2]), matrix[2, 0]), op.mul(op.mul(matrix[0, 1], matrix[1, 0]), matrix[2, 2]))),
            op.sub(op.mul(op.mul(matrix[0, 2], matrix[1, 0]), matrix[2, 1]), op.mul(op.mul(matrix[0, 2], matrix[1, 1]), matrix[2, 0])));
        T ans = op.zero;
        for (int x = 0, i = 1; x < n; x++, i = -i)
            ans = op.sum(op.mul(op.convert(i), op.mul(matrix[x, 0], Minor(x, 0))), ans);
        return ans;
    }
    public Matrix<T> Transparent()
    {
        T[,] tmp = new T[m, n];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[y, x] = matrix[x, y];
        return new Matrix<T>(m, n, tmp);

    }
    public Matrix<T> Adjugate()
    {
        T[,] tmp = new T[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = Addition(x, y);
        return new Matrix<T>(n, m, tmp);
    }
    public Matrix<T> Invert()
    {
        return Adjugate().Transparent() / Determinant();
    }
    public T permanent()
    {
        if (n == 1) return matrix[0, 0];
        if (n == 2) return op.sum(op.mul(matrix[0, 0], matrix[1, 1]), op.mul(matrix[0, 1], matrix[1, 0]));

        T ans = op.zero;
        for (int x = 0; x < n; x++)
            ans = op.sum(ans, op.mul(matrix[x, 0], AddMinor(x, 0).permanent()));
        return ans;
    }
    public T Permanent()
    {
        if (!Square) throw new Exception();
        return permanent();
    }

    public static Matrix<T> Horizontal(T[] v)
    {
        int m = v.Length;
        var tmp = new T[1, m];
        for (int i = 0; i < m; i++)
            tmp[0, i] = v[i];
        return new Matrix<T>(1, m, tmp);
    }
    public static Matrix<T> Vertical(T[] v)
    {
        int n = v.Length;
        var tmp = new T[n, 1];
        for (int i = 0; i < n; i++)
            tmp[i, 0] = v[i];
        return new Matrix<T>(n, 1, tmp);
    }
}

public class Matrix
{
    protected double[,] matrix;
    protected int n, m;
    public int N => n;
    public int M => m;

    protected Matrix(int N, int M, double[,] Matrix) { n = N; m = M; matrix = Matrix; }
    public Matrix(double[,] Matrix) : this(Matrix.GetLength(0), Matrix.GetLength(1), Matrix) { }
    public Matrix(int N, int M) : this(N, M, new double[N, M]) { }
    public Matrix(Matrix Matrix) : this(Matrix.matrix.Clone() as double[,]) { }

    public object Clone() => new Matrix(this);

    public double this[int x, int y] { get { return matrix[x, y]; } set { matrix[x, y] = value; } }

    public bool Square => n == m;
    public static bool Like(Matrix a, Matrix b) => a.n == b.n && a.m == b.m;

    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (!Like(a, b)) throw new Exception();
        int n = a.n, m = a.m;
        var tmp = new double[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = a.matrix[x, y] + b.matrix[x, y];
        return new Matrix(n, m, tmp);
    }
    public static Matrix operator -(Matrix a, Matrix b)
    {
        if (!Like(a, b)) throw new Exception();
        int n = a.n, m = a.m;
        var tmp = new double[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = a.matrix[x, y] - b.matrix[x, y];
        return new Matrix(n, m, tmp);
    }
    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.m != b.n) throw new Exception();
        double[,] tmp = new double[a.n, b.m];
        double sum;
        for (int x = a.n - 1; x >= 0; --x)
        {
            for (int y = b.m - 1; y >= 0; --y)
            {
                sum = 0;
                for (int i = a.m - 1; i >= 0; --i)
                {
                    sum += a.matrix[x, i] * b.matrix[i, y];
                }
                tmp[x, y] = sum;
            }
        }
        return new Matrix(tmp);
    }
    public static Matrix operator *(Matrix a, double v)
    {
        int n = a.n, m = a.m;
        var tmp = new double[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = a.matrix[x, y] * v;
        return new Matrix(n, m, tmp);
    }
    public static Matrix operator /(Matrix a, double v)
    {
        int n = a.n, m = a.m;
        var tmp = new double[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = a.matrix[x, y] / v;
        return new Matrix(n, m, tmp);
    }

    public Matrix AddMinor(int cutX, int cutY)
    {
        var tmp = new double[n - 1, m - 1];
        for (int x = 0, xx = 0; xx < n - 1; x++, xx++)
        {
            if (x == cutX) x++;
            for (int y = 0, yy = 0; yy < m - 1; y++, yy++)
            {
                if (y == cutY) y++;
                tmp[xx, yy] = matrix[x, y];
            }
        }
        return new Matrix(n - 1, m - 1, tmp);
    }
    public double Minor(int x, int y) => AddMinor(x, y).Determinant();
    public double Addition(int x, int y) => Minor(x, y) * (((x + y + 1) & 1) * 2 - 1);
    public double Determinant()
    {
        if (n != m || n <= 0) throw new Exception();
        if (n == 1) return matrix[0, 0];
        if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        if (n == 3)
            return
            matrix[0, 0] * matrix[1, 1] * matrix[2, 2] - matrix[0, 0] * matrix[1, 2] * matrix[2, 1] +
            matrix[0, 1] * matrix[1, 2] * matrix[2, 0] - matrix[0, 1] * matrix[1, 0] * matrix[2, 2] +
            matrix[0, 2] * matrix[1, 0] * matrix[2, 1] - matrix[0, 2] * matrix[1, 1] * matrix[2, 0];
        double ans = 0;
        for (int x = 0, i = 1; x < n; x++, i = -i)
            ans += i * matrix[x, 0] * Minor(x, 0);
        return ans;
    }
    public Matrix Transparent()
    {
        double[,] tmp = new double[m, n];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[y, x] = matrix[x, y];
        return new Matrix(m, n, tmp);

    }
    public Matrix Adjugate()
    {
        double[,] tmp = new double[n, m];
        for (int x = 0; x < n; x++)
            for (int y = 0; y < m; y++)
                tmp[x, y] = Addition(x, y);
        return new Matrix(n, m, tmp);
    }
    public Matrix Invert()
    {
        return Adjugate().Transparent() / Determinant();
    }
    public double permanent()
    {
        if (n == 1) return matrix[0, 0];
        if (n == 2) return matrix[0, 0] * matrix[1, 1] + matrix[0, 1] * matrix[1, 0];

        double ans = 0;
        for (int x = 0; x < n; x++)
            ans += matrix[x, 0] * AddMinor(x, 0).permanent();
        return ans;
    }
    public double Permanent()
    {
        if (!Square) throw new Exception();
        return permanent();
    }

    public static Matrix Horizontal(double[] v)
    {
        int m = v.Length;
        var tmp = new double[1, m];
        for (int i = 0; i < m; i++)
            tmp[0, i] = v[i];
        return new Matrix(1, m, tmp);
    }
    public static Matrix Vertical(double[] v)
    {
        int n = v.Length;
        var tmp = new double[n, 1];
        for (int i = 0; i < n; i++)
            tmp[i, 0] = v[i];
        return new Matrix(n, 1, tmp);
    }
}