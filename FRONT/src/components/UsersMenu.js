import React, { useState } from 'react';

function UsersMenu({ users, onNavigate, onSendMessage }) {
  const [showAll, setShowAll] = useState(false);
  const [showSendForm, setShowSendForm] = useState(false);
  const [userId, setUserId] = useState('');
  const [message, setMessage] = useState('');
  const [statusMessage, setStatusMessage] = useState('');

  const handleSendMessage = async () => {
    if (!userId || !message) {
      setStatusMessage('Заполните все поля!');
      return;
    }

    try {
      const success = await onSendMessage(parseInt(userId), message);
      if (success) {
        setStatusMessage('Сообщение отправлено!');
        setMessage('');
        setUserId('');
        setTimeout(() => setStatusMessage(''), 3000);
      } else {
        setStatusMessage('Ошибка отправки или пользователь не найден');
      }
    } catch (error) {
      setStatusMessage('Ошибка отправки или пользователь не найден');
    }
  };

  return (
    <div className="menu-container">
      <h1 className="menu-title">Пользователи</h1>
      
      <div className="menu-buttons">
        <button className="menu-btn" onClick={() => setShowAll(!showAll)}>
          1. Показать всех
        </button>
        <button className="menu-btn" onClick={() => setShowSendForm(!showSendForm)}>
          2. Отправить сообщение пользователю
        </button>
        <button className="menu-btn secondary" onClick={() => onNavigate('main')}>
          3. Назад
        </button>
      </div>

      {showAll && (
        <div className="content-section">
          <h3>Все пользователи:</h3>
          <div className="user-list">
            {users.map(user => (
              <div key={user.id} className="user-item">
                [{user.id}] {user.name} ({user.cityNow})
              </div>
            ))}
          </div>
        </div>
      )}

      {showSendForm && (
        <div className="content-section">
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

export default UsersMenu;
