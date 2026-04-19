import toast from 'react-hot-toast';

/**
 * Показує toast помилки лише якщо це не тиха (silent) помилка.
 * Тихими вважаються 401 відповіді від API — для гостя це нормально.
 * @param {object|string} error - об'єкт помилки або рядок
 * @param {string} fallback - запасне повідомлення якщо error.message відсутнє
 */
export const toastError = (error, fallback = 'Сталася помилка') => {
    if (error?.isSilent) return;
    const message = (typeof error === 'string' ? error : error?.message) || fallback;
    if (message) toast.error(message);
};
