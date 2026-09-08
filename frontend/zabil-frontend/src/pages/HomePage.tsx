import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import Hero from "../components/home/Hero";
import AboutSection from "../components/home/AboutSection";
import { endSession } from "../lib/authSession";
import { useAuth } from "../lib/useAuth";

const HomePage = () => {
  const { isLoggedIn, isAdmin } = useAuth();

  const onSignOut = () => endSession("signed-out");

  return (
    <div className="flex min-h-screen flex-col">
      <Header isLoggedIn={isLoggedIn} isAdmin={isAdmin} onSignOut={onSignOut} />
      <main className="flex-1">
        <Hero />
        <AboutSection />
      </main>
      <Footer />
    </div>
  );
};

export default HomePage;
