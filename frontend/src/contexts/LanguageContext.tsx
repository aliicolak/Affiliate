import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { type Language, getTranslation, languages } from '../i18n';

interface LanguageContextType {
    language: Language;
    setLanguage: (lang: Language) => void;
    t: (key: string) => string;
    languages: typeof languages;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

export const useLanguage = () => {
    const context = useContext(LanguageContext);
    if (context === undefined) {
        throw new Error('useLanguage must be used within a LanguageProvider');
    }
    return context;
};

interface LanguageProviderProps {
    children: ReactNode;
}

export const LanguageProvider = ({ children }: LanguageProviderProps) => {
    const [language, setLanguageState] = useState<Language>(() => {
        const stored = localStorage.getItem('affiliate_language') as Language;
        if (stored && languages.some(l => l.code === stored)) return stored;

        // Detect browser language
        const browserLang = navigator.language.split('-')[0];
        const supported = languages.find(l => l.code === browserLang);
        return supported ? supported.code : 'en';
    });

    useEffect(() => {
        localStorage.setItem('affiliate_language', language);
        document.documentElement.setAttribute('lang', language);
        // RTL support for Arabic
        document.documentElement.setAttribute('dir', language === 'ar' ? 'rtl' : 'ltr');
    }, [language]);

    const setLanguage = (lang: Language) => {
        setLanguageState(lang);
    };

    const t = (key: string): string => {
        return getTranslation(language, key);
    };

    return (
        <LanguageContext.Provider value={{ language, setLanguage, t, languages }}>
            {children}
        </LanguageContext.Provider>
    );
};

export default LanguageContext;
