import React, { useState, useEffect } from 'react';
import './App.css';
import MainMenu from './components/MainMenu';
import RegionsMenu from './components/RegionsMenu';
import UsersMenu from './components/UsersMenu';
import SendWarningMenu from './components/SendWarningMenu';
import LogsMenu from './components/LogsMenu';
import * as api from './handlers/api';

function App() {
  const [currentView, setCurrentView] = useState('main');
  const [users, setUsers] = useState([]);
  const [cities, setCities] = useState({});

  useEffect(() => {
    fetchUsers();
    fetchCities();
  }, []);

  const fetchUsers = async () => {
    try {
      console.log('Загрузка пользователей...');
      const data = await api.getAllUsers();
      console.log('Получено пользователей:', data);
      setUsers(data);
    } catch (error) {
      console.error('Ошибка загрузки пользователей:', error);
    }
  };

  const fetchCities = async () => {
    try {
      console.log('Загрузка статистики городов...');
      const data = await api.getCityStatistics();
      console.log('Получено городов:', data);
      setCities(data);
    } catch (error) {
      console.error('Ошибка загрузки статистики городов:', error);
    }
  };

  const sendMessageToCity = async (city, message) => {
    try {
      await api.sendMessageToCity(city, message);
      return true;
    } catch (error) {
      console.error('Ошибка отправки:', error);
      return false;
    }
  };

  const sendMessageToUser = async (userId, message) => {
    try {
      await api.sendMessageToUser(userId, message);
      return true;
    } catch (error) {
      console.error('Ошибка отправки:', error);
      return false;
    }
  };

  const renderView = () => {
    switch (currentView) {
      case 'main':
        return <MainMenu onNavigate={setCurrentView} />;
      case 'regions':
        return <RegionsMenu cities={cities} users={users} onNavigate={setCurrentView} onSendMessage={sendMessageToCity} />;
      case 'users':
        return <UsersMenu users={users} onNavigate={setCurrentView} onSendMessage={sendMessageToUser} />;
      case 'sendWarning':
        return <SendWarningMenu users={users} onNavigate={setCurrentView} onSendMessageToCity={sendMessageToCity} onSendMessageToUser={sendMessageToUser} />;
      case 'logs':
        return <LogsMenu onBack={() => setCurrentView('main')} />;
      default:
        return <MainMenu onNavigate={setCurrentView} />;
    }
  };

  return (
    <div className="App">
      {renderView()}
    </div>
  );
}

export default App;
