import React from 'react';

class ErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
        this.state = { hasError: false, error: null };
    }

    static getDerivedStateFromError(error) {
        return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
        console.error("Uncaught error:", error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return (
                <div className="page-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '80vh', textAlign: 'center' }}>
                    <div className="glass-panel" style={{ padding: '3rem', maxWidth: '600px' }}>
                        <h1 style={{ color: '#ef4444', marginBottom: '1rem' }}>Упс! Щось пішло не так</h1>
                        <p style={{ color: 'var(--text-muted)', marginBottom: '2rem' }}>
                            Вибачте, сталася неочікувана помилка. Ми вже працюємо над її вирішенням.
                        </p>
                        <div style={{ background: 'rgba(0,0,0,0.2)', padding: '1rem', borderRadius: '8px', marginBottom: '2rem', textAlign: 'left', overflowX: 'auto' }}>
                            <code style={{ fontSize: '0.8rem', color: '#ff7070' }}>
                                {this.state.error?.toString()}
                            </code>
                        </div>
                        <button 
                            className="btn btn-primary" 
                            onClick={() => window.location.reload()}
                        >
                            Оновити сторінку
                        </button>
                    </div>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
