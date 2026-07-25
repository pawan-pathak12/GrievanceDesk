import { ArrowRight, BellRing, ClipboardCheck, MessageSquareText, ShieldCheck } from 'lucide-react';
import { Link } from 'react-router-dom';
import { ROUTES } from '../utils/routes';
import '../Styles/landing.css';

const workflow = [
  {
    icon: ClipboardCheck,
    title: 'Report with context',
    text: 'Members submit categorized complaints with the details administrators need to begin work.',
  },
  {
    icon: MessageSquareText,
    title: 'Keep the conversation clear',
    text: 'A shared discussion and activity history keep every update connected to the complaint.',
  },
  {
    icon: BellRing,
    title: 'Close the loop',
    text: 'Track each complaint through review and resolution while keeping members informed.',
  },
];

export default function LandingPage() {
  return (
    <main className="landing-page">
      <section className="landing-hero">
        <nav className="landing-nav" aria-label="Primary navigation">
          <Link className="landing-brand" to="/" aria-label="GrievanceDesk home">
            <span className="landing-brand-mark">GD</span>
            <span>GrievanceDesk</span>
          </Link>

          <div className="landing-nav-actions">
            <Link className="landing-login-link" to={ROUTES.LOGIN}>Sign in</Link>
            <Link className="landing-nav-cta" to={ROUTES.REGISTER_EMAIL}>Create account</Link>
          </div>
        </nav>

        <div className="landing-hero-content">
          <p className="landing-eyebrow">Complaint management for organizations</p>
          <h1>GrievanceDesk</h1>
          <p className="landing-hero-statement">A clear, accountable path from concern to resolution.</p>
          <p className="landing-hero-copy">
            Give members a secure place to report issues and give your team one focused workspace
            to review, discuss, and resolve them.
          </p>
          <div className="landing-hero-actions">
            <Link className="landing-primary-button" to={ROUTES.REGISTER_EMAIL}>
              Get started <ArrowRight size={17} aria-hidden="true" />
            </Link>
            <Link className="landing-secondary-button" to={ROUTES.LOGIN}>Sign in to workspace</Link>
          </div>
        </div>

        <div className="landing-hero-footer" aria-label="Platform highlights">
          <span><ShieldCheck size={16} aria-hidden="true" /> Secure member access</span>
          <span><ClipboardCheck size={16} aria-hidden="true" /> Trackable resolution workflow</span>
          <span><MessageSquareText size={16} aria-hidden="true" /> Shared discussion history</span>
        </div>
      </section>

      <section className="landing-intro" aria-labelledby="landing-intro-title">
        <div>
          <p className="landing-section-label">One reliable record</p>
          <h2 id="landing-intro-title">Keep reporting, communication, and action in one place.</h2>
        </div>
        <p>
          GrievanceDesk replaces scattered follow-ups with a simple workflow that gives members
          visibility and gives administrators the context to respond with confidence.
        </p>
      </section>

      <section className="landing-workflow" aria-label="GrievanceDesk workflow">
        {workflow.map(({ icon: Icon, title, text }, index) => (
          <article className="landing-workflow-item" key={title}>
            <div className="landing-workflow-icon"><Icon size={21} aria-hidden="true" /></div>
            <span className="landing-workflow-number">0{index + 1}</span>
            <h3>{title}</h3>
            <p>{text}</p>
          </article>
        ))}
      </section>

      <section className="landing-capabilities" aria-labelledby="landing-capabilities-title">
        <div className="landing-capabilities-heading">
          <p className="landing-section-label">Built for accountable operations</p>
          <h2 id="landing-capabilities-title">Everything needed to keep a complaint moving.</h2>
        </div>
        <ul className="landing-capability-list">
          <li><ShieldCheck size={18} aria-hidden="true" /> Role-based member and administrator workspaces</li>
          <li><ClipboardCheck size={18} aria-hidden="true" /> Status tracking, activity history, and resolution notes</li>
          <li><MessageSquareText size={18} aria-hidden="true" /> Complaint discussions with clear ownership and moderation</li>
          <li><BellRing size={18} aria-hidden="true" /> Organization notices for timely updates</li>
        </ul>
      </section>

      <section className="landing-final-cta" aria-labelledby="landing-cta-title">
        <div>
          <p className="landing-section-label">Start today</p>
          <h2 id="landing-cta-title">Give every concern a visible path forward.</h2>
        </div>
        <Link className="landing-primary-button landing-final-button" to={ROUTES.REGISTER_EMAIL}>
          Create an account <ArrowRight size={17} aria-hidden="true" />
        </Link>
      </section>

      <footer className="landing-footer">
        <span>GrievanceDesk</span>
        <span>Complaint management for organizations that listen and act.</span>
      </footer>
    </main>
  );
}
