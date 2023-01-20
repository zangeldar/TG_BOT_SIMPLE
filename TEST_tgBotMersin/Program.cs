// See https://aka.ms/new-console-template for more information
using TEST_tgBotMersin;
using tgBotMersin;
using tglib;

Console.WriteLine("Hello, World!");

string GoogleSheetCellsUrlExample = "https://sheets.googleapis.com/v4/spreadsheets/1tO5HIjEKPeZdITzzdO7OyuD_iPcuyVF-z9-ITIA0Jw4/";

dynamic test;
dynamic tmp;

//test = new testTG();
//tmp = test.DoTest();

test = new testGSheet();
tmp = "";
//tmp = test.DoTest();
tmp = test.ProceedDate();



Console.WriteLine(tmp);








