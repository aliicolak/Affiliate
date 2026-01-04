import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from './AuthContext';
import { getAccessToken } from '../api/client';
import type { Notification } from '../types';

interface SignalRContextType {
    connection: signalR.HubConnection | null;
    notifications: Notification[];
    unreadCount: number;
}

const SignalRContext = createContext<SignalRContextType | null>(null);

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({ children }: { children: React.ReactNode }) => {
    const { isAuthenticated } = useAuth();
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);

    useEffect(() => {
        if (!isAuthenticated) {
            if (connection) {
                connection.stop();
                setConnection(null);
            }
            return;
        }

        const getBaseUrl = () => {
            let url = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
            if (url.endsWith('/api')) {
                url = url.substring(0, url.length - 4);
            }
            if (url.endsWith('/')) {
                url = url.substring(0, url.length - 1);
            }
            return url;
        };

        const hubUrl = `${getBaseUrl()}/notifications/hub`;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => getAccessToken() || ''
            })
            .withAutomaticReconnect()
            .build();

        newConnection.on("ReceiveNotification", (notification: Notification) => {
            console.log("Notification received:", notification);
            setNotifications(prev => [notification, ...prev]);
            setUnreadCount(prev => prev + 1);
        });

        newConnection.start()
            .then(() => console.log('SignalR Connected to ' + hubUrl))
            .catch(err => console.error('SignalR Connection Error: ', err));

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, [isAuthenticated]);

    return (
        <SignalRContext.Provider value={{ connection, notifications, unreadCount }}>
            {children}
        </SignalRContext.Provider>
    );
};
