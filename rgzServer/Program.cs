// SocketServer.cs
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Data.SqlClient;

namespace rgzServer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int del = 0;

            /*
            Socket Класс предоставляет широкий набор методов и свойств для сетевых взаимодействий. 
            Socket Класс позволяет выполнять синхронный и асинхронную передачу данных с использованием 
            любого из коммуникационных протоколов, перечисленных в ProtocolType перечисления.
            */

            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

       // Создаем сокет Tcp/Ip. TCP/IP — сетевая модель передачи данных, представленных в цифровом виде. 
       //Модель описывает способ передачи данных от источника информации к получателю.
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // ожидание клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    del = Convert.ToInt32(data, 16);
                    bd(del);
                    Console.Write("\n");
                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");

                    // Отправляем ответ клиенту
                    string reply = "Спасибо за запрос в " + data.Length.ToString() + " символов";
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }

            void bd(int delete)
            {
                //путь к файлу с бд
                const string databaseName = @"D:\Bank_deposits2.db";
                //подключение к бд
                SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
                connection.Open();
                //sqlite комагда
                SQLiteCommand command = new SQLiteCommand("DELETE FROM bank WHERE id_bank = " + delete + "; SELECT * FROM 'bank';", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                //вывод данных из таблицы
                Console.Write("\u250C" + new string('\u2500', 12) + "\u252C" + new string('\u2500', 60) + "\u2510");
                Console.WriteLine("\n\u2502" + "номер счета \u2502" + new string(' ', 21) + "Дата посещения" + new string(' ', 25) + "\u2502");
                Console.Write("\u251C" + new string('\u2500', 12) + "\u253C" + new string('\u2500', 60) + "\u2524\n");
                foreach (DbDataRecord record in reader)
                {
                    string id = record["id_bank"].ToString();
                    id = id.PadLeft(12 - id.Length, ' ');
                    string value = record["date_of_visit"].ToString();
                    string result = "\u2502" + id + " \u2502";
                    value = value.PadLeft(60, ' ');
                    result += value + "\u2502";
                    Console.WriteLine(result);
                }
                Console.Write("\u2514" + new string('\u2500', 12) + "\u2534" + new string('\u2500', 60) + "\u2518");
               
                connection.Close();
               // Console.ReadKey(true);
               
            } 
        }
    }

}