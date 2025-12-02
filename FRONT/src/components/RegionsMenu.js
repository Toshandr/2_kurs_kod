import React, { useState } from 'react';

function RegionsMenu({ cities, users, onNavigate, onSendMessage }) {
  const [selectedCity, setSelectedCity] = useState('');
  const [message, setMessage] = useState('');
  const [showUsers, setShowUsers] = useState(false);
  const [showSendForm, setShowSendForm] = useState(false);
  const [statusMessage, setStatusMessage] = useState('');

  const sortedCities = Object.entries(cities).sort((a, b) => b[1] - a[1]);

  const handleShowUsers = () => {
    setShowUsers(true);
    setShowSendForm(false);
  };

  const handleShowSendForm = () => {
    setShowSendForm(true);
    setShowUsers(false);
  };

  const handleSendMessage = async () => {
    if (!selectedCity || !message) {
      setStatusMessage('Заполните все поля!');
      return;
    }

    try {
      const success = await onSendMessage(selectedCity, message);
      if (success) {
        setStatusMessage('Сообщение отправлено!');
        setMessage('');
        setTimeout(() => setStatusMessage(''), 3000);
      } else {
        setStatusMessage('Ошибка отправки');
      }
    } catch (error) {
      setStatusMessage('Ошибка отправки');
    }
  };

  const usersInCity = users.filter(u => u.cityNow === selectedCity);

  return (
    <div className="menu-container">
      <h1 className="menu-title">Регионы</h1>
      
      <div className="content-section">
        <h3>Пользователи по городам:</h3>
        <div className="city-list">
          {sortedCities.map(([city, count]) => (
            <div key={city} className="city-item">
              <strong>{city}</strong>: {count} пользователей
            </div>
          ))}
        </div>
      </div>

      <div className="menu-buttons">
        <button className="menu-btn" onClick={handleShowUsers}>
          1. Просмотреть пользователей города
        </button>
        <button className="menu-btn" onClick={handleShowSendForm}>
          2. Отправить предупреждение в город
        </button>
        <button className="menu-btn secondary" onClick={() => onNavigate('main')}>
          3. Назад
        </button>
      </div>

      {showUsers && (
        <div className="content-section">
          <div className="input-group">
            <label>Название города:</label>
            <input
              type="text"
              value={selectedCity}
              onChange={(e) => setSelectedCity(e.target.value)}
              placeholder="Введите город"
            />
          </div>
          {selectedCity && (
            <div className="user-list">
              {usersInCity.length > 0 ? (
                usersInCity.map(user => (
                  <div key={user.id} className="user-item">
                    <div><strong>ID:</strong> {user.id}</div>
                    <div><strong>Имя:</strong> {user.name}</div>
                    <div><strong>Возраст:</strong> {user.age}</div>
                    <div><strong>Роль:</strong> {user.role || 'guest'}</div>
                  </div>
                ))
              ) : (
                <p>Пользователи не найдены</p>
              )}
            </div>
          )}
        </div>
      )}

      {showSendForm && (
        <div className="content-section">
          <div className="input-group">
            <label>Название города:</label>
            <input
              type="text"
              value={selectedCity}
              onChange={(e) => setSelectedCity(e.target.value)}
              placeholder="Введите город"
            />
          </div>
          <div className="input-group">
            <label>Текст предупреждения:</label>
            <textarea
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              placeholder="Введите сообщение"
            />
          </div>
          <button className="menu-btn" onClick={handleSendMessage}>
            Отправить
          </button>
          {statusMessage && (
            <div className={statusMessage.includes('Ошибка') ? 'error-message' : 'success-message'}>
              {statusMessage}
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default RegionsMenu;
