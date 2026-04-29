import toast from 'react-hot-toast';

/**
 
 * @param {object|string} error
 * @param {string} fallback 
 */
export const toastError = (error, fallback = 'Сталася помилка') => {
    if (error?.isSilent) return;
    const message = (typeof error === 'string' ? error : error?.message) || fallback;
    if (message) toast.error(message);
};
