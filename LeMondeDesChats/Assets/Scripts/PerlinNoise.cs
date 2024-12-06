using System;

public class PerlinNoise
{
    private int[] permutationTable;

    // Constructeur
    public PerlinNoise(int seed)
    {
        permutationTable = new int[512];
        var random = new Random(seed);

        // Génère une table de permutation
        for (int i = 0; i < 256; i++)
        {
            permutationTable[i] = i;
        }

        // Mélange la table de permutation
        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            int temp = permutationTable[i];
            permutationTable[i] = permutationTable[j];
            permutationTable[j] = temp;
        }

        // Copie la table pour avoir 512 valeurs
        for (int i = 0; i < 256; i++)
        {
            permutationTable[256 + i] = permutationTable[i];
        }
    }

    // Fonction pour interpoler entre deux valeurs
    private static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // Fonction pour calculer un produit scalaire entre deux vecteurs
    private static double Dot(int[] grad, double x, double y)
    {
        return grad[0] * x + grad[1] * y;
    }

    // Fonction principale pour calculer le bruit de Perlin 2D
    public double Noise(double x, double y)
    {
        // Calculer les indices des coins
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        // Déterminer les décalages dans chaque dimension
        double xf = x - Math.Floor(x);
        double yf = y - Math.Floor(y);

        // Calculer la fade (interpolation) dans chaque direction
        double u = Fade(xf);
        double v = Fade(yf);

        // Générer les coins du carré
        int aa = permutationTable[X + permutationTable[Y]] % 12;
        int ab = permutationTable[X + permutationTable[Y + 1]] % 12;
        int ba = permutationTable[X + 1 + permutationTable[Y]] % 12;
        int bb = permutationTable[X + 1 + permutationTable[Y + 1]] % 12;

        // Calculer les gradients
        double gradAA = Dot(Gradients(aa), xf, yf);
        double gradAB = Dot(Gradients(ab), xf, yf - 1);
        double gradBA = Dot(Gradients(ba), xf - 1, yf);
        double gradBB = Dot(Gradients(bb), xf - 1, yf - 1);

        // Interpoler entre les gradients
        double x1 = Lerp(gradAA, gradBA, u);
        double x2 = Lerp(gradAB, gradBB, u);
        return Lerp(x1, x2, v);
    }

    // Fonction d'interpolation linéaire
    private static double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    // Fonction pour obtenir un gradient à partir de la table de permutation
    private static int[] Gradients(int hash)
    {
        switch (hash & 15)
        {
            case 0: return new int[] { 1, 1 };
            case 1: return new int[] { -1, 1 };
            case 2: return new int[] { 1, -1 };
            case 3: return new int[] { -1, -1 };
            case 4: return new int[] { 1, 0 };
            case 5: return new int[] { -1, 0 };
            case 6: return new int[] { 0, 1 };
            case 7: return new int[] { 0, -1 };
            case 8: return new int[] { 1, 0 };
            case 9: return new int[] { -1, 0 };
            case 10: return new int[] { 0, 1 };
            case 11: return new int[] { 0, -1 };
            default: return new int[] { 1, 1 };
        }
    }
}