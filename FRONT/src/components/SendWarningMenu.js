import React, { useState } from 'react';

function SendWarningMenu({ users, onNavigate, onSendMessageToCity, onSendMessageToUser }) {
  const [mode, setMode] = useState('');
  const [userId, setUserId] = useState('');
  const [city, setCity] = useState('');
  const [message, setMessage] = useState('');
  const [statusMessage, setStatusMessage] = useState('');

  const handleSendToUser = async () => {
    if (!userId || !message) {
      setStatusMessage('Заполните все поля!');
      return;
    }

    try {
      const success = await onSendMessageToUser(parseInt(userId), message);
      if (success) {
        setStatusMessage('Сообщение отправлено!');
        setMessage('');
        setUserId('');
        setTimeout(() => setStatusMessage(''), 3000);
      } else {
        setStatusMessage('Ошибка отправки');
      }
    } catch (error) {
      setStatusMessage('Ошибка отправки');
    }
  };

  const handleSendToCity = async () => {
    if (!city || !message) {
      setStatusMessage('Заполните все поля!');
      return;
    }

    try {
      const success = await onSendMessageToCity(city, message);
      if (success) {
        setStatusMessage('Сообщение отправлено!');
        setMessage('');
        setCity('');
        setTimeout(() => setStatusMessage(''), 3000);
      } else {
        setStatusMessage('Ошибка отправки');
      }
    } catch (error) {
      setStatusMessage('Ошибка отправки');
    }
  };

  return (
    <div className="menu-container">
      <h1 className="menu-title">Отправить предупреждение</h1>
      
      <div className="menu-buttons">
        <button className="menu-btn" onClick={() => setMode('user')}>
          1. Отправить пользователю
        </button>
        <button className="menu-btn" onClick={() => setMode('city')}>
          2. Отправить в город
        </button>
        <button className="menu-btn secondary" onClick={() => onNavigate('main')}>
          3. Назад
        </button>
      </div>

      {mode === 'user' && (
        <div className="content-section">
          <h3>Отправить пользователю</h3>
          <div className="input-group">
            <label>ID пользователя:</label>
            <input
              type="number"
              value={userId}
              onChange={(e) => setUserId(e.target.value)}
              placeholder="Введите ID"
            />
          </div>
          <div className="input-group">
            <label>Сообщение:</label>
            <textarea
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              placeholder="Введите сообщение"
            />
          </div>
          <button className="menu-btn" onClick={handleSendToUser}>
            Отправить
          </button>
          {statusMessage && (
            <div className={statusMessage.includes('Ошибка') ? 'error-message' : 'success-message'}>
              {statusMessage}
            </div>
          )}
        </div>
      )}

      {mode === 'city' && (
        <div className="content-section">
          <h3>Отправить в город</h3>
          <div className="input-group">
            <label>Название города:</label>
            <input
              type="text"
              value={city}
              onChange={(e) => setCity(e.target.value)}
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
          <button className="menu-btn" onClick={handleSendToCity}>
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

export default SendWarningMenu;
