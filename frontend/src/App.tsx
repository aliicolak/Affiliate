import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import { AuthProvider, ThemeProvider, LanguageProvider, SignalRProvider, useAuth } from './contexts';
import { Navbar } from './components';
import { Login, Register, Dashboard, Offers, Commissions, Notifications, BlogList, BlogDetail, BlogEditor, SocialFeed, ShareDetail, UserProfilePage, ShareEditor, Collections, CollectionDetail } from './pages';
import './index.css';

// Protected route wrapper
const ProtectedRoute = () => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="page">
        <div className="container">
          <div className="loading-state" style={{ minHeight: '50vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <div className="spinner"></div>
          </div>
        </div>
      </div>
    );
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

// Guest route wrapper
const GuestRoute = () => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="page">
        <div className="loading-state" style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <div className="spinner"></div>
        </div>
      </div>
    );
  }

  return isAuthenticated ? <Navigate to="/dashboard" replace /> : <Outlet />;
};

// Main Layout
const Layout = () => {
  return (
    <>
      <Navbar />
      <Outlet />
    </>
  );
};

const App = () => {
  return (
    <BrowserRouter>
      <ThemeProvider>
        <LanguageProvider>
          <AuthProvider>
            <SignalRProvider>
              <Routes>
                <Route element={<Layout />}>
                  {/* Guest only routes */}
                  <Route element={<GuestRoute />}>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                  </Route>

                  {/* Public routes */}
                  <Route path="/blog" element={<BlogList />} />
                  <Route path="/blog/:slug" element={<BlogDetail />} />
                  <Route path="/social" element={<SocialFeed />} />
                  <Route path="/social/share/:id" element={<ShareDetail />} />
                  <Route path="/profile/:userId" element={<UserProfilePage />} />

                  {/* Protected routes */}
                  <Route element={<ProtectedRoute />}>
                    <Route path="/dashboard" element={<Dashboard />} />
                    <Route path="/offers" element={<Offers />} />
                    <Route path="/commissions" element={<Commissions />} />
                    <Route path="/notifications" element={<Notifications />} />
                    <Route path="/collections" element={<Collections />} />
                    <Route path="/collections/:id" element={<CollectionDetail />} />
                    <Route path="/blog/new" element={<BlogEditor />} />
                    <Route path="/blog/edit/:id" element={<BlogEditor />} />
                    <Route path="/social/new" element={<ShareEditor />} />
                  </Route>

                  {/* Default redirect */}
                  <Route path="/" element={<Navigate to="/blog" replace />} />
                  <Route path="*" element={<Navigate to="/blog" replace />} />
                </Route>
              </Routes>
            </SignalRProvider>
          </AuthProvider>
        </LanguageProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
};

export default App;
