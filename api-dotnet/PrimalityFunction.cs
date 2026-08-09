using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Numerics;
using System.Security.Cryptography;

namespace ApiDotnet;

public class PrimalityFunction
{
    // Not a correctness limit — BigInteger has none — just a guard against pathological
    // input (e.g. a million-digit string) tying up a free-tier function.
    private const int MaxDigits = 2000;

    // Above this, exact smallest-factor search via trial division stops being fast.
    private static readonly BigInteger SmallestFactorSearchLimit = 1_000_000_000_000_000; // 10^15

    [Function("CheckPrimality")]
    public IActionResult Check(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "primality/{n}")] HttpRequest req,
        string n)
    {
        if (n.TrimStart('-').Length > MaxDigits)
            return new BadRequestObjectResult(new { error = $"Number must not exceed {MaxDigits} digits." });

        if (!BigInteger.TryParse(n, out var number))
            return new BadRequestObjectResult(new { error = $"'{n}' is not a valid integer." });

        if (number < 0)
            return new BadRequestObjectResult(new { error = "Number must not be negative." });

        var result = PrimalityChecker.Evaluate(number);
        BigInteger? smallestFactor = !result.IsPrime && number >= 2 && number <= SmallestFactorSearchLimit
            ? PrimalityChecker.SmallestFactor(number)
            : null;

        return new OkObjectResult(new
        {
            number = number.ToString(),
            isPrime = result.IsPrime,
            method = result.Method,
            smallestFactor = smallestFactor?.ToString()
        });
    }
}

public readonly record struct PrimalityResult(bool IsPrime, string Method);

public static class PrimalityChecker
{
    // Deterministic for all n below this bound — no randomness needed, exact answer every time.
    private static readonly BigInteger DeterministicBound = BigInteger.Parse("3317044064679887385961981");
    private static readonly int[] SmallPrimes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37];

    // Rounds used only beyond DeterministicBound — probability of a false positive is <= 4^-40,
    // the same standard used by widely-used crypto libraries for "certain in practice" primality.
    private const int ProbabilisticRounds = 40;
    private const string TrialDivisionMethod = "trial division";
    private const string DeterministicMethod = "deterministic Miller-Rabin";
    private static readonly string ProbabilisticMethod = $"probabilistic Miller-Rabin ({ProbabilisticRounds} rounds)";

    public static PrimalityResult Evaluate(BigInteger n)
    {
        if (n < 2) return new PrimalityResult(false, TrialDivisionMethod);

        foreach (var p in SmallPrimes)
        {
            if (n == p) return new PrimalityResult(true, TrialDivisionMethod);
            if (n % p == 0) return new PrimalityResult(false, TrialDivisionMethod);
        }

        var d = n - 1;
        var r = 0;
        while (d % 2 == 0)
        {
            d /= 2;
            r++;
        }

        if (n < DeterministicBound)
        {
            foreach (var a in SmallPrimes)
            {
                if (!MillerRabinPasses(n, d, r, a))
                    return new PrimalityResult(false, DeterministicMethod);
            }
            return new PrimalityResult(true, DeterministicMethod);
        }

        using var rng = RandomNumberGenerator.Create();
        for (var i = 0; i < ProbabilisticRounds; i++)
        {
            var a = RandomInRange(2, n - 2, rng);
            if (!MillerRabinPasses(n, d, r, a))
                return new PrimalityResult(false, ProbabilisticMethod);
        }
        return new PrimalityResult(true, ProbabilisticMethod);
    }

    private static bool MillerRabinPasses(BigInteger n, BigInteger d, int r, BigInteger a)
    {
        var x = BigInteger.ModPow(a, d, n);
        if (x == 1 || x == n - 1) return true;

        for (var i = 0; i < r - 1; i++)
        {
            x = BigInteger.ModPow(x, 2, n);
            if (x == n - 1) return true;
        }

        return false;
    }

    private static BigInteger RandomInRange(BigInteger min, BigInteger max, RandomNumberGenerator rng)
    {
        var range = max - min;
        var bytes = range.ToByteArray();
        BigInteger result;
        do
        {
            var buf = new byte[bytes.Length + 1]; // extra byte guarantees non-negative sign
            rng.GetBytes(buf, 0, bytes.Length);
            result = new BigInteger(buf);
        } while (result > range || result < 0);
        return min + result;
    }

    // Only called for n <= SmallestFactorSearchLimit, so trial division up to sqrt(n) stays fast.
    public static BigInteger SmallestFactor(BigInteger n)
    {
        if (n % 2 == 0) return 2;
        for (BigInteger i = 3; i * i <= n; i += 2)
        {
            if (n % i == 0) return i;
        }
        return n; // n itself is prime — unreachable given IsPrime already ruled that out
    }
}
