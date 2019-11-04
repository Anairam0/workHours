using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;


namespace Horas_de_trabajo
{
    class Program
    {
        static void Main(string[] args)
        {
            int workHours = Convert.ToInt32(Console.ReadLine().Trim());

            int dayHours = Convert.ToInt32(Console.ReadLine().Trim());

            string pattern = Console.ReadLine();

            List<string> result = Result.findSchedules(workHours, dayHours, pattern);

            Console.WriteLine(string.Join("\n", result));

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}

class Result
{
    public static List<string> findSchedules(int workHours, int dayHours, string pattern)
    {
        var temp = TempVariables(pattern, dayHours, workHours);

        var combinaciones = findCombinations(temp, workHours, dayHours, pattern);

        return combinaciones;
    }

    public static TempVariables TempVariables(string pattern, int dayHours, int weekHours)
    {
        TempVariables temp = new TempVariables();

        int baseMaximo = dayHours;
        temp.HorasRestantes = weekHours;
        int exponente = 0;


        for (var i = 0; i < Constants.PatternLenght; i++)
        {
            var actualCaracter = pattern.Substring(i, 1);

            if (actualCaracter == "?")
            {
                temp.ValorMaximo += baseMaximo * Convert.ToInt32(Math.Pow(10, exponente));
                temp.ValorMinimo += 0;
                exponente++;
                temp.NumberOfQuestions++;
            }
            else
            {
                temp.HorasRestantes -= int.Parse(actualCaracter);
            }
        }
        return temp;
    }

    public static string createPattern(string pattern, string auxPattern)
    {
        var arrayOriginal = pattern;
        var arrayAux = auxPattern;
        int indiceAux = 0;

        string aux = string.Empty;
        for (int i = 0; i < Constants.PatternLenght; i++)
        {
            if (arrayOriginal[i] == '?')
            {
                aux += arrayAux[indiceAux];

                indiceAux++;
            }
            else
            {
                aux += arrayOriginal[i];
            }
        }
        return aux;
    }


    public static List<string> findCombinations(TempVariables patternInfo, int workHours, int dayHours, string originalPattern)
    {
        int auxiliarPattern = patternInfo.ValorMinimo;

        List<string> patrones = new List<string>();

        while (auxiliarPattern <= patternInfo.ValorMaximo)
        {
            var stringPattern = auxiliarPattern.ToString().PadLeft(patternInfo.NumberOfQuestions, '0');

            if (CheckPattern(stringPattern, dayHours, patternInfo.HorasRestantes))
            {
                patrones.Add(createPattern(originalPattern, stringPattern));
            }
            auxiliarPattern = auxiliarPattern + 1;
        }
        return patrones;
    }


    public static bool CheckPattern(string pattern, int dayHours, int remaHours)
    {
        var totalSuma = 0;
        //Verifying max hours per day
        for (var i = 0; i < pattern.Length; i++)
        {
            if (int.Parse(pattern[i].ToString()) > dayHours)
            {
                return false;
            }
            totalSuma += int.Parse(pattern.Substring(i, 1));
        }
        //Verifying remaining hours in week
        if (totalSuma == remaHours)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

class TempVariables
{
    public TempVariables()
    {
        ValorMaximo = 0;
        ValorMinimo = 0;
        NumberOfQuestions = 0;
        NumberPattern = new List<int>();
    }

    public int ValorMaximo { get; set; }
    public int ValorMinimo { get; set; }
    public int HorasRestantes { get; set; }
    public int NumberOfQuestions { get; set; }
    public List<int> NumberPattern { get; set; }
}

static class Constants
{
    public const int PatternLenght = 7;
    public const string Mask = "0000000";
}
