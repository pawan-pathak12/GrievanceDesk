import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { complaintApi } from '../../api/complaintApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { CategoryOptions } from '../../utils/enums';

export default function NewComplaintPage() {
  const [form, setForm] = useState({ title: '', description: '', category: '' });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!form.title || !form.description || form.category === '') return;

    setLoading(true);
    const res = await complaintApi.create({
      title: form.title,
      description: form.description,
      category: parseInt(form.category),
    });
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Complaint submitted successfully');
      navigate(ROUTES.MY_COMPLAINTS);
    } else {
      toast.error(res.errors?.[0] || 'Failed to submit complaint');
    }
  }

  return (
    <div className="complaint-form-page">
      <Link to={ROUTES.MY_COMPLAINTS} className="back-link">← Back to List</Link>

      <div className="page-header">
        <h1>Submit New Complaint</h1>
        <p>Please provide clear details to help us resolve the issue efficiently.</p>
      </div>

      <form onSubmit={handleSubmit}>
        <div className="form-card">
          <div className="form-group">
            <label>Complaint Title</label>
            <input
              type="text"
              name="title"
              placeholder="Briefly summarize the issue..."
              value={form.title}
              onChange={handleChange}
              required
              maxLength={200}
            />
            <div className="form-hint">Example: Course Registration Portal Connectivity Issue</div>
          </div>

          <div className="form-group">
            <label>Category</label>
            <select name="category" value={form.category} onChange={handleChange} required>
              <option value="">Select a category</option>
              {CategoryOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Full Description</label>
            <textarea
              name="description"
              placeholder="Provide as much detail as possible, including dates, locations, and involved parties..."
              value={form.description}
              onChange={handleChange}
              required
              maxLength={2000}
            />
            <div className="form-hint">Maximum 2000 characters · {form.description.length}/2000</div>
          </div>
        </div>

        <div className="form-footer">
          <button type="button" className="btn-cancel" onClick={() => navigate(ROUTES.MY_COMPLAINTS)}>
            Cancel
          </button>
          <button type="submit" className="btn-submit" disabled={loading}>
            {loading ? 'Submitting...' : '▶ Submit Complaint'}
          </button>
        </div>
      </form>
    </div>
  );
}
