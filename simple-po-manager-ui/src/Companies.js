import React, { useEffect, useState } from 'react';
import { supabase } from './supabaseClient';

function Companies() {
  const [companies, setCompanies] = useState([]);

  useEffect(() => {
    async function fetchCompanies() {
      const { data, error } = await supabase
        .from('companies')
        .select('*');
      if (error) {
        alert('Error fetching companies: ' + error.message);
      } else {
        setCompanies(data);
      }
    }
    fetchCompanies();
  }, []);

  return (
    <div>
      <h2>Companies (from Supabase)</h2>
      <ul>
        {companies.map(company => (
          <li key={company.id}>
            {company.name} - {company.address}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default Companies;
