using System.Text.Json;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using System.Net.Http.Json;

bool menu = true;
bool switchMenu = true;
int mainMenu;
int dictionIndex;
string json;

List<MyDictionary>? Diction = new List<MyDictionary>();

try
{
    string[] dictionaryPathArray = Directory.GetFiles(Directory.GetCurrentDirectory(), "*-*.json");

    for (int i = 0; i < dictionaryPathArray.Length; i++)
    {
        for (int j = dictionaryPathArray[i].Length - 6; j > 0; j--)
        {
            if (dictionaryPathArray[i][j] == '\\')
            {
                string dictionaryName = dictionaryPathArray[i].Remove(0, j + 1);
                dictionaryName = dictionaryName.Substring(0, dictionaryName.Length - 5);
                Diction.Add(new MyDictionary(dictionaryName, JsonConvert.DeserializeObject<List<Word>>(File.ReadAllText(dictionaryPathArray[i]))));
                break;
            }
        }
    }
}
catch (Exception)
{
    Console.WriteLine("Ошибка чтения файла");
}

string word, translate;

while (menu)
{
    while (true)
    {
        Console.WriteLine("Введите свой выбор");
        Console.WriteLine("1 - Добавить словарь");
        Console.WriteLine("2 - Работать с словарем");
        Console.WriteLine("0 - Выход");

        if (int.TryParse(Console.ReadLine(), out mainMenu) && mainMenu >= 0 && mainMenu <= 2)
            break;
        else
            Console.WriteLine("Ошибка");
    }

    Console.Clear();
    if (mainMenu == 0) break;

    if (mainMenu == 1)
    {
        Console.WriteLine("Введите назвиние для соваря:");
        word = Console.ReadLine() ?? "";
        if (word.Length > 0)
            Diction.Add(new MyDictionary(word));
        else
            Console.WriteLine("Ошибка");
    }

    else if (mainMenu == 2)
    {
        if (Diction.Count == 0)
        {
            Console.WriteLine("Словарей нет");
            continue;
        }

        while (true)
        {
            Console.WriteLine("Выберите словарь");

            for (int i = 0; i < Diction.Count; i++)
            {
                Console.WriteLine($"{i} - {Diction[i].Name}");
            }

            if (int.TryParse(Console.ReadLine(), out dictionIndex) && dictionIndex >= 0 && dictionIndex < Diction.Count)
                break;
            else
                Console.WriteLine("Ошибка");
        }
        while (switchMenu)
        {
            while (true)
            {
                Console.WriteLine("\nВведите необходимый пункт");
                Console.WriteLine("1 - Добавить слово в словарь");
                Console.WriteLine("2 - Добавить перевод к слову в словаре");
                Console.WriteLine("3 - Удалить слово из словаря");
                Console.WriteLine("4 - Удалить один из вариантов перевода слова");
                Console.WriteLine("5 - Изменить слово в словаре");
                Console.WriteLine("6 - Искать слово в словаре по одной или несколько буквам");
                Console.WriteLine("7 - Искать слово по переводу");
                Console.WriteLine("8 - Экспортировать слово в файл");
                Console.WriteLine("0 - Выход");

                if (int.TryParse(Console.ReadLine(), out mainMenu) && mainMenu >= 0 && mainMenu <= 8)
                    break;
                else
                    Console.WriteLine("Ошибка");
            }
            switch ((Menu)mainMenu)
            {
                case Menu.Exit:
                    switchMenu = false;
                    menu = false;
                    break;

                case Menu.AddWord:
                    Console.WriteLine("Введите слово");
                    word = Console.ReadLine() ?? "";
                    Console.WriteLine("Введите перевод слова");
                    translate = Console.ReadLine() ?? "";

                    Diction[dictionIndex].AddWord(word, translate);
                    break;

                case Menu.AddTranslate:
                    Console.WriteLine("Введите слово что бы добавить перевод");
                    word = Console.ReadLine() ?? "";
                    Console.WriteLine("Введите перевод слова");
                    translate = Console.ReadLine() ?? "";

                    if (Diction[dictionIndex].AddTranslate(word, translate))
                        Console.WriteLine("Запись добавлена");
                    else
                        Console.WriteLine("Ошибка");

                    break;

                case Menu.RemoveWord:
                    Console.WriteLine("Введите слово и мы удалим его из словаря");

                    if (Diction[dictionIndex].RemoveWord(Console.ReadLine() ?? ""))
                        Console.WriteLine("Запись удалена");
                    else
                        Console.WriteLine("Ошибка");

                    break;

                case Menu.RemoveTranslate:
                    Console.WriteLine("Введите слово, перевод которого хотите удалить");
                    word = Console.ReadLine() ?? "";
                    Console.WriteLine("Введите перевод который удалим");
                    translate = Console.ReadLine() ?? "";

                    if (Diction[dictionIndex].RemoveTranslate(word, translate))
                        Console.WriteLine("Запись удалена");
                    else
                        Console.WriteLine("Ошибка");

                    break;

                case Menu.ChangeWord:
                    Console.WriteLine("Введите слово что заменяем");
                    word = Console.ReadLine() ?? "";
                    Console.WriteLine("Введите новое слово");

                    if (Diction[dictionIndex].ChangeWord(word, Console.ReadLine() ?? ""))
                        Console.WriteLine("Запись изменена");
                    else
                        Console.WriteLine("Ошибка");

                    break;
                case Menu.SearchWord:
                    Console.WriteLine("Введите слово которое будете искать");
                    Console.WriteLine("Слово должно начинатся с большой буквы");
                    Diction[dictionIndex].PrintWords(Console.ReadLine() ?? "");

                    break;

                case Menu.SearchTranslate:
                    Console.WriteLine("Введите перевод для поиска в словаре");

                    Word result = Diction[dictionIndex].GetWord(Diction[dictionIndex].SearchTranslate(Console.ReadLine() ?? ""));

                    if (result.OriginalWord != "0")
                        result.ShowWord();
                    else
                        Console.WriteLine("Такого нет");

                    break;

                case Menu.ExportWord:
                    Console.WriteLine("Введите слово которое запишем в файл");
                    int index = Diction[dictionIndex].SearchWord(Console.ReadLine() ?? "");

                    if (index == -1)
                    {
                        Console.WriteLine("Такого нет");
                        break;
                    }

                    Word wordForJson = Diction[dictionIndex].GetWord(index);


                    Console.WriteLine("Введите куда будем мы записывать это");
                    string pathWordExport = Console.ReadLine() ?? "export_word";
                    pathWordExport += ".json";

                    json = JsonConvert.SerializeObject(wordForJson);
                    File.WriteAllText(pathWordExport, json);

                    break;

                default:
                    break;
            }
        }
    }

    if (!menu)
    {
        for (int i = 0; i < Diction?.Count; i++)
            Diction[i].Serialize(i);

        Console.WriteLine("Данные сохранены успешно");
    }

}

public enum Menu
{
    Exit, AddWord, AddTranslate, RemoveWord, RemoveTranslate, ChangeWord, SearchWord, SearchTranslate, ExportWord
}

public class Word
{
    public List<string> translations = new List<string>();
    public string OriginalWord { get; set; }

    public Word(string originalWord, string translate)
    {
        OriginalWord = originalWord;
        translations.Add(translate);
    }

    public bool AddTranslate(string translate)
    {
        if (translate.Length == 0)
            return false;
        foreach (var item in translations)
        {
            if (item == translate)
                return false;
        }

        translations.Add(translate);
        return true;
    }

    public void ShowWord()
    {
        Console.WriteLine($"Оригинальное слово: {OriginalWord}");
        Console.WriteLine("Перевод:");

        foreach (var item in translations)
            Console.WriteLine(item);
    }

    public bool ChangeTranslate(string oldTranslate, string newTranslate)
    {
        int index = translations.IndexOf(oldTranslate);
        if (index == -1)
            return false;

        translations[index] = newTranslate;
        return true;
    }

    public bool RemoveTranslate(string translate)
    {
        if (translations.Count < 2)
            return false;

        return translations.Remove(translate);
    }
}

public class MyDictionary
{
    List<Word> words;
    public string Name { get; set; }

    public MyDictionary(string name)
    {
        words = new List<Word>();
        Name = name;
    }
    public MyDictionary(string name, List<Word> list)
    {
        if (list != null)
            words = list;
        else
            words = new List<Word>();

        Name = name;
    }
    public void Serialize(int index)
    {
        File.WriteAllText($"{Name}.json", JsonConvert.SerializeObject(words));
    }
    public int SearchWord(string word)
    {
        if (word.Length == 0)
            return -1;

        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].OriginalWord == word)
                return i;
        }

        return -1;
    }

    public int SearchTranslate(string translate)
    {
        if (translate.Length == 0)
            return -1;

        for (int i = 0; i < words.Count; i++)
        {
            foreach (var item in words[i].translations)
            {
                if (item == translate)
                    return i;
            }
        }

        return -1;
    }

    public bool AddWord(string originalWord, string translate)
    {
        if (originalWord.Length == 0 || translate.Length == 0)
            return false;

        words.Add(new Word(originalWord, translate));
        return true;
    }

    public bool AddTranslate(string originalWord, string translate)
    {
        if (originalWord.Length == 0 || translate.Length == 0)
            return false;

        int index = SearchWord(originalWord);
        if (index < 0) return false;

        return words[index].AddTranslate(translate);
    }

    public bool ChangeWord(string old_originalWord, string new_originalWord)
    {
        if (old_originalWord.Length == 0 || new_originalWord.Length == 0)
            return false;

        int index = SearchWord(old_originalWord);
        if (index < 0) return false;

        words[index].OriginalWord = new_originalWord;
        return true;
    }

    public bool RemoveWord(string originalWord)
    {
        int index = SearchWord(originalWord);
        if (index < 0) return false;

        words.RemoveAt(index);
        return true;
    }

    public bool RemoveTranslate(string originalWord, string translate)
    {
        int index = SearchWord(originalWord);
        if (index < 0) return false;

        return words[index].RemoveTranslate(translate);
    }

    public Word GetWord(int index)
    {
        if (index < 0 || index >= words.Count)
            return new Word("0", "0");

        return words[index];
    }

    public bool PrintWords(string partOfWord)
    {
        bool result = false;
        foreach (var item in words)
        {
            if (item.OriginalWord.StartsWith(partOfWord))
            {
                item.ShowWord();
                result = true;
            }
        }

        return result;
    }
}