namespace WL.Host.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// Попытка получения данных из кэша
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Возвращаемое значение</param>
        /// <param name="isTouched">Флаг указывающий обновиться ли время жизни объекта (default(false))</param>
        bool GetValue<TKey, TValue>(TKey key, out TValue value, bool isTouched = false);

        /// <summary>
        /// Получить коллекцию значений из кэша
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        IEnumerable<TValue> GetValues<TKey, TValue>();

        /// <summary>
        /// Попытка добавить данные в кэш
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value);

        /// <summary>
        /// Попытка добавить данные в кэш с обновлённым периодом актуальности
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        /// <param name="timeToExpire">Время жизни</param>
        TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire);

        /// <summary>
        /// Попытка добавить данные в кэш с методом генерирование нового объекта при существовании key
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        /// <param name="updateValue">Метод генерирование нового объекта при существовании key</param>
        TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, Func<TKey, TValue, TValue> updateValue);

        /// <summary>
        /// Попытка добавить данные в кэш с указанным временем жизни или обновления данных в кэше с обновлением времени жизни
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        /// <param name="timeToExpire">Время жизни</param>
        /// <param name="updateValue">Метод генерирование нового объекта при существовании key</param>
        TValue AddOrUpdate<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire, Func<TKey, TValue, TValue> updateValue);

        /// <summary>
        /// Попытка добавить данные в кэш с указанным временем жизни или обновления данных в кэше без обновления времени жизни
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        /// <param name="timeToExpire">Время жизни нового объекта</param>
        /// <param name="updateValue">Метод генерирование нового объекта при существовании key</param>
        TValue AddOrUpdateNoTouch<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire, Func<TKey, TValue, TValue> updateValue);

        /// <summary>
        /// Добавляет значение, если ранее значение с таким ключом отсутствовало
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        bool TryAdd<TKey, TValue>(TKey key, TValue value);

        /// <summary>
        /// Добавляет значение с указанным временем жизни, если ранее значение с таким ключом отсутствовало
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="key">Ключ</param>
        /// <param name="value">Добавляемое значение</param>
        /// <param name="timeToExpire">Время жизни</param>
        bool TryAdd<TKey, TValue>(TKey key, TValue value, TimeSpan timeToExpire);

        /// <summary>
        /// Безопасное удаление из кэша
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        bool Remove<TKey, TValue>(TKey key, out TValue value);

        /// <summary>
        /// Получить количество элементов кэша в связке ключ-значение
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип значения</typeparam>
        int Count<TKey, TValue>();

        /// <summary>
        /// Получить количество элементов всего кэша
        /// </summary>
        long CountAll();
    }
}
