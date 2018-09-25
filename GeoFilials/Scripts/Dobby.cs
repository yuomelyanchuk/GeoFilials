using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GeoFilials.Scripts
{
    public static class Dobby
    {
        #region Поля
        public static string lastFilterParams;
        #endregion

        #region int
        /// <summary>
        /// Получить Индекс кнопки, по имени.
        /// </summary>
        /// <param name="ButtonName"></param>
        /// <returns></returns>
        public static int GetButtonIndex(string ButtonName)
        {
            int val;
            if (Int32.TryParse(Regex.Replace(ButtonName, @"[A-Z]|[a-z]|[А-Я]|[а-я]", string.Empty), out val))
                return val;
            else
                return 0;
        }
        #endregion

        #region string
        /// <summary>
        /// Получить текст для диалога.
        /// </summary>
        /// <param name="DialogID"></param>
        /// <returns></returns>
        public static string GetDialogText(int DialogID)
        {
            switch (DialogID)
            {
                case 1:
                    return "Добавленно";
                case 2:
                    return "Изменения внесены успешно";
                case 3:
                    return "Готово!";
                case 4:
                    return "Укажите Тип Бизнеса";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Получить текст ошибки.
        /// </summary>
        /// <param name="ErrorID"></param>
        /// <returns></returns>
        public static string GetErrorText(int ErrorID)
        {
            switch (ErrorID)
            {
                case 1:
                    return "Год не может быть меньше 1991";
                case 2:
                    return "Введен некорректный формат\nПоле \'Год\' принимает формат : ГГГГ";
                case 3:
                    return "Значение \'Месяц\' не может привышать 12";
                case 4:
                    return "Значение \'Число\' не может привышать 31";
                case 5:
                    return "Введент некорректный формат\nПоле \'Дата открытия\'\nпринимает формат : ДД.ММ.";
                case 6:
                    return "Филиал уже существует";
                case 7:
                    return "Выбран бизнесс \"Fozzy Group\"\nФилиал введен не корректно!";
                case 8:
                    return "Вводимое значение, не является координатами";
                case 9:
                    return "Область не выбрана!";
                case 10:
                    return "Редактирование не завершено";
                case 11:
                    return "Фильтр не установлен!";
                case 12:
                    return "Филиал не выбран";
                case 13:
                    return "Не удалось создать файл";
                case 14:
                    return "Не выбран Тип Бизнеса";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Получить строку ФИЛЬТРА. Для отображения данных.
        /// </summary>
        /// <param name="boxes"></param>
        /// <returns></returns>
        public static string GetFilterString(ref List<ComboBox> boxes)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ComboBox box in boxes)
            {
                builder.Append(string.Format("{0} = \'{1}\' AND ", box.Tag.ToString(), box.SelectedItem.ToString()));
            }
            lastFilterParams = builder.ToString().Substring(0, builder.Length - 4);
            return lastFilterParams;
        }

        /// <summary>
        /// Перегрузка метода GetFilterString()
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public static void GetFilterString(Dictionary<string, string> keyValuePairs)
        {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, string> item in keyValuePairs)
            {
                builder.Append(string.Format("{0} = \'{1}\' AND ", item.Key, item.Value));
            }
            lastFilterParams = builder.ToString().Substring(0, builder.Length - 4);
        }

        /// <summary>
        /// Получить имя для файла с данными для карты.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMapFileName(int index)
        {
            switch (index)
            {
                case 1:
                    return "Все филиалы продовольственные.kml";
                case 2:
                    return "Все филиалы непродовольственные.kml";
                case 3:
                    return "Сильпо.kml";
                case 4:
                    return "Все филиалы аптеки.kml";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Получить текст, для шапки MessageBox.
        /// </summary>
        /// <param name="TitleID"></param>
        /// <returns></returns>
        public static string GetMessageBoxTitle(int TitleID)
        {
            switch (TitleID)
            {
                case 1:
                    return "Ошибка";
                case 2:
                    return "Просьба сделать криншот";
                case 3:
                    return "Внимание!";
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
