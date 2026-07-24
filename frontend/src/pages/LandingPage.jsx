import { Link } from 'react-router-dom';
import { ROUTES } from '../utils/routes';
import '../Styles/landing.css';

const highlights = [
  {
    number: '01',
    title: 'Submit clearly',
    text: 'Share the issue, category, and context your organization needs to act on it.',
  },
  {
    number: '02',
    title: 'Follow every update',
    text: 'See your complaint move from review to resolution with a clear activity trail.',
  },
  {
    number: '03',
    title: 'Resolve together',
    text: 'Give administrators the tools to review, prioritize, and close the loop.',
  },
];

export default function LandingPage() {
  return (
    <main className="landing-page">
      <section className="landing-hero">
        <nav className="landing-nav" aria-label="Primary navigation">
          <Link className="landing-brand" to="/">
            <span className="landing-brand-mark">GD</span>
            <span>GrievanceDesk</span>
          </Link>

          <div className="landing-nav-actions">
            <Link className="landing-login-link" to={ROUTES.LOGIN}>Sign in</Link>
            <Link className="landing-nav-cta" to={ROUTES.REGISTER_EMAIL}>Create account</Link>
          </div>
        </nav>

        <div className="landing-hero-content">
          <p className="landing-eyebrow">Organization operations, made accountable</p>
          <h1>Make every concern easier to hear and resolve.</h1>
          <p className="landing-hero-copy">
            GrievanceDesk gives members a simple way to report issues and gives administrators
            one reliable workspace to respond with clarity.
          </p>
          <div className="landing-hero-actions">
            <Link className="landing-primary-button" to={ROUTES.REGISTER_EMAIL}>Get started</Link>
            <Link className="landing-secondary-button" to={ROUTES.LOGIN}>Sign in to workspace</Link>
          </div>
        </div>

        <div className="landing-hero-footer">
          <span>Built for transparent organization operations</span>
          <span>Secure member access</span>
          <span>Trackable resolutions</span>
        </div>
      </section>

      <section className="landing-intro">
        <div className="landing-section-heading">
          <p className="landing-eyebrow">One shared record</p>
          <h2>From first report to final resolution.</h2>
        </div>
        <p>
          Replace scattered messages and uncertain follow-ups with a focused workflow that keeps
          members informed and teams aligned.
        </p>
      </section>

      <section className="landing-highlights" aria-label="How GrievanceDesk works">
        {highlights.map((highlight) => (
          <article className="landing-highlight" key={highlight.number}>
            <span className="landing-highlight-number">{highlight.number}</span>
            <h3>{highlight.title}</h3>
            <p>{highlight.text}</p>
          </article>
        ))}
      </section>

      <section className="landing-final-cta">
        <div>
          <p className="landing-eyebrow">Start with a clearer conversation</p>
          <h2>Your next report deserves a visible path forward.</h2>
        </div>
        <Link className="landing-primary-button" to={ROUTES.REGISTER_EMAIL}>Create your account</Link>
      </section>

      <footer className="landing-footer">
        <span>GrievanceDesk</span>
        <span>Complaint management for organizations that listen and act.</span>
      </footer>
    </main>
  );
}
